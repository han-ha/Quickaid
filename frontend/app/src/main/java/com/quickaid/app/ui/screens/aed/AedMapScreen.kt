package com.quickaid.app.ui.screens.aed

import android.Manifest
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.location.LocationManager
import android.os.Handler
import android.os.Looper
import android.provider.Settings
import androidx.activity.compose.BackHandler
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.ComposeView
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.viewinterop.AndroidView
import androidx.core.app.ActivityCompat
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.AedDto
import com.quickaid.app.enums.AedType
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AedViewModel
import com.quickaid.app.viewmodel.SessionViewModel
import org.osmdroid.config.Configuration
import org.osmdroid.util.GeoPoint
import org.osmdroid.views.MapView
import org.osmdroid.views.overlay.Marker
import org.osmdroid.views.overlay.infowindow.InfoWindow
import org.osmdroid.views.overlay.mylocation.GpsMyLocationProvider
import org.osmdroid.views.overlay.mylocation.MyLocationNewOverlay

@Composable
fun AedMapScreen(
    navController: NavController,
    viewModel: AedViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    // Kontekst aplikacji oraz stany z ViewModeli
    val context = LocalContext.current
    val aeds by viewModel.aeds.collectAsState()
    val userRole by sessionViewModel.role.collectAsState()

    // Sprawdzenie zezwolenia na lokalizację
    var hasLocationPermission by remember {
        mutableStateOf(
            ActivityCompat.checkSelfPermission(
                context,
                Manifest.permission.ACCESS_FINE_LOCATION
            ) == PackageManager.PERMISSION_GRANTED
        )
    }

    // Launcher do pytania użytkownika o pozwolenie na lokalizację
    val permissionLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.RequestPermission()
    ) { granted -> hasLocationPermission = granted }

    // Stany pomocnicze do obsługi dialogu, okienka ze szczegółami AED i centrowania mapy
    var showEnableLocationDialog by remember { mutableStateOf(false) }
    var lastOpenInfoWindow: InfoWindow? by remember { mutableStateOf(null) }
    var isCenteredOnUser by remember { mutableStateOf(false) }

    // Funkcja sprawdzająca, czy lokalizacja jest włączona w ustawieniach systemowych
    fun isLocationEnabled(context: Context): Boolean {
        val locationManager = context.getSystemService(Context.LOCATION_SERVICE) as LocationManager
        return locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER) ||
                locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER)
    }

    // Funkcja otwierająca ekran ustawień lokalizacji w systemie Android
    fun openLocationSettings() {
        val intent = Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
        context.startActivity(intent)
    }

    // Pobranie AED i sprawdzenie zezwolenia na lokalizację
    LaunchedEffect(Unit) {
        viewModel.fetchAeds()
        if (!hasLocationPermission) {
            permissionLauncher.launch(Manifest.permission.ACCESS_FINE_LOCATION)
        } else if (!isLocationEnabled(context)) {
            showEnableLocationDialog = true
        }
    }

    // Obsługa przycisku cofania: zamyka InfoWindow albo wraca do poprzedniego ekranu
    BackHandler {
        lastOpenInfoWindow?.close() ?: navController.popBackStack()
    }

    // Tworzymy mapę z domyślnym centrum - Warszawa
    val mapView = remember {
        Configuration.getInstance().userAgentValue = context.packageName
        MapView(context).apply {
            setMultiTouchControls(true)
            controller.setZoom(15.0)
            controller.setCenter(GeoPoint(52.2297, 21.0122)) // Warszawa domyślnie
            setOnTouchListener { _, _ ->
                lastOpenInfoWindow?.close()
                false
            }
        }
    }

    // InfoWindow dla markerów AED
    fun createInfoWindow(aed: AedDto): ComposeView {
        return ComposeView(context).apply {
            setContent {
                Card(
                    shape = MaterialTheme.shapes.medium,
                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
                    modifier = Modifier.padding(AppSizes.extraSmall)
                ) {
                    Column(
                        modifier = Modifier.padding(AppSizes.small),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Text(
                            text = aed.description ?: "AED",
                            style = MaterialTheme.typography.titleSmall,
                            maxLines = 3,
                            overflow = TextOverflow.Ellipsis
                        )
                        Text(
                            text = if (aed.verified) "Zweryfikowany" else "Niezweryfikowany",
                            style = MaterialTheme.typography.bodySmall,
                            maxLines = 1,
                            overflow = TextOverflow.Ellipsis
                        )

                        Spacer(modifier = Modifier.height(AppSizes.small))

                        // Przycisk edycji widoczny tylko dla AED z bazy danych
                        if (aed.type == AedType.Internal) {
                            SmallButton(
                                onClick = {
                                    viewModel.selectAed(aed)
                                    navController.navigate("editAed")
                                },
                                content = "Edytuj"
                            )
                        }
                    }
                }
            }
        }
    }

    // Funkcja aktualizacji markerów AED
    fun updateMarkers(map: MapView, aeds: List<AedDto>) {
        map.post {
            map.overlays.removeAll { it is Marker }
            lastOpenInfoWindow?.close()

            aeds.forEach { aed ->
                val marker = Marker(map).apply {
                    position = GeoPoint(aed.latitude, aed.longitude)
                    title = aed.description ?: "AED"
                    subDescription = if (aed.verified) "Zweryfikowany" else "Niezweryfikowany"
                    setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM)

                    // Obsługa kliknięcia markera - otwieranie InfoWindow
                    setOnMarkerClickListener { m, _ ->
                        lastOpenInfoWindow?.close()
                        val info = object : InfoWindow(createInfoWindow(aed), map) {
                            override fun onOpen(item: Any?) { lastOpenInfoWindow = this }
                            override fun onClose() { if (lastOpenInfoWindow == this) lastOpenInfoWindow = null }
                        }
                        info.open(m, m.position, 0, -m.icon.intrinsicHeight)
                        true
                    }
                }
                map.overlays.add(marker)
            }
            map.invalidate()
        }
    }

    // Reakcja na zmianę listy AED - odświeżenie markerów na mapie
    LaunchedEffect(aeds) {
        updateMarkers(mapView, aeds)
    }

    Box(modifier = Modifier.fillMaxSize()) {

        // Dialog proszący użytkownika o włączenie lokalizacji w ustawieniach systemowych
        if (showEnableLocationDialog) {
            AlertDialog(
                onDismissRequest = {},
                title = { Text("Udostępnij lokalizację") },
                text = { Text("Aby sprawdzić swoją pozycję na mapie, włącz lokalizację w ustawieniach.") },
                confirmButton = {
                    SmallButton(
                        onClick = {
                            openLocationSettings()
                            showEnableLocationDialog = false
                        },
                        content = "OK"
                    )
                },
                dismissButton = {
                    SmallButton(
                        onClick = { showEnableLocationDialog = false },
                        content = "Anuluj"
                    )
                }
            )
        }

        // Widok mapy
        AndroidView(
            factory = { mapView },
            modifier = Modifier.fillMaxSize(),
            update = { map ->
                if (hasLocationPermission && isLocationEnabled(context)) {
                    val locationProvider = GpsMyLocationProvider(context)
                    val myLocationOverlay = MyLocationNewOverlay(locationProvider, map)
                    myLocationOverlay.enableMyLocation()

                    // Centrowanie mapy na użytkowniku przy pierwszym fixie GPS
                    myLocationOverlay.runOnFirstFix {
                        val loc = myLocationOverlay.myLocation
                        if (loc != null && !isCenteredOnUser) {
                            Handler(Looper.getMainLooper()).post {
                                map.controller.animateTo(loc)
                                isCenteredOnUser = true
                            }
                        }
                    }
                    if (!map.overlays.contains(myLocationOverlay)) {
                        map.overlays.add(myLocationOverlay)
                    }
                }
            }
        )

        // Przycisk dodawania AED - widoczny tylko dla użytkowników zalogowanych
        if (userRole != UserRole.ANON) {
            CustomIconButton(
                onClick = { navController.navigate("addAed") },
                modifier = Modifier
                    .align(Alignment.TopEnd)
                    .padding(end = AppSizes.extraLarge * 2, top = AppSizes.medium)
                    .size(AppSizes.extraLarge),
                icon = Icons.Filled.Add,
                contentDescription = "Dodaj punkt AED"
            )
        }

        // Przycisk powrotu do ekranu głównego
        CustomIconButton(
            onClick = {
                navController.navigate("home") {
                    popUpTo(navController.graph.startDestinationId) { inclusive = true }
                }
            },
            modifier = Modifier
                .align(Alignment.TopEnd)
                .padding(AppSizes.medium)
                .size(AppSizes.extraLarge),
            icon = Icons.Filled.Home,
            contentDescription = "Powrót do ekranu głównego"
        )
    }
}
