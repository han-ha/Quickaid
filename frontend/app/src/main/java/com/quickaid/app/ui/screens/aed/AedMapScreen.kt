package com.quickaid.app.ui.screens.aed

import android.Manifest
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.Color
import android.location.LocationManager
import android.os.Handler
import android.os.Looper
import android.provider.Settings
import android.util.TypedValue
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import androidx.activity.compose.BackHandler
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.cardview.widget.CardView
import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.viewinterop.AndroidView
import androidx.core.app.ActivityCompat
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.AedDto
import com.quickaid.app.enums.AedType
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AedViewModel
import com.quickaid.app.viewmodel.SessionViewModel
import org.osmdroid.config.Configuration
import org.osmdroid.events.MapListener
import org.osmdroid.events.ScrollEvent
import org.osmdroid.events.ZoomEvent
import org.osmdroid.util.BoundingBox
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
    val context = LocalContext.current
    val aeds by viewModel.aeds.collectAsState()
    val userRole by sessionViewModel.role.collectAsState()

    var hasLocationPermission by remember {
        mutableStateOf(
            ActivityCompat.checkSelfPermission(
                context,
                Manifest.permission.ACCESS_FINE_LOCATION
            ) == PackageManager.PERMISSION_GRANTED
        )
    }

    val permissionLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.RequestPermission()
    ) { granted -> hasLocationPermission = granted }

    var isCenteredOnUser by remember { mutableStateOf(false) }
    var showEnableLocationDialog by remember { mutableStateOf(false) }
    var lastOpenInfoWindow: InfoWindow? by remember { mutableStateOf(null) }

    fun isLocationEnabled(context: Context): Boolean {
        val locationManager = context.getSystemService(Context.LOCATION_SERVICE) as LocationManager
        return locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER) ||
                locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER)
    }

    fun openLocationSettings() {
        val intent = Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
        context.startActivity(intent)
    }

    LaunchedEffect(Unit) {
        viewModel.fetchAeds()
        if (!hasLocationPermission) {
            permissionLauncher.launch(Manifest.permission.ACCESS_FINE_LOCATION)
        } else if (!isLocationEnabled(context)) {
            showEnableLocationDialog = true
        }
    }

    BackHandler {
        lastOpenInfoWindow?.close() ?: navController.popBackStack()
    }

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

    fun createInfoWindow(aed: AedDto): CardView {
        return CardView(context).apply {
            radius = TypedValue.applyDimension(
                TypedValue.COMPLEX_UNIT_DIP,
                8f,
                context.resources.displayMetrics
            )
            setCardBackgroundColor(Color.WHITE)
            setContentPadding(16, 16, 16, 16)
            layoutParams = LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT
            )
            addView(LinearLayout(context).apply {
                orientation = LinearLayout.VERTICAL
                val titleView = TextView(context).apply {
                    text = aed.description ?: "AED"
                    setTextColor(Color.BLACK)
                    setTextSize(TypedValue.COMPLEX_UNIT_SP, 16f)
                }
                val descView = TextView(context).apply {
                    text = if (aed.verified) "Zweryfikowany" else "Niezweryfikowany"
                    setTextColor(Color.DKGRAY)
                    setTextSize(TypedValue.COMPLEX_UNIT_SP, 14f)
                }
                addView(titleView)
                addView(descView)

                if (userRole != UserRole.ANON && aed.type == AedType.Internal) {
                    val editBtn = Button(context).apply {
                        text = "Edytuj"
                        setOnClickListener {
                            viewModel.selectAed(aed)
                            navController.navigate("editAed")
                        }
                    }
                    addView(editBtn)
                }
            })
        }
    }

    fun updateMarkers(map: MapView, aeds: List<AedDto>) {
        val bounds: BoundingBox = map.boundingBox
        map.overlays.removeAll { it is Marker }

        aeds.forEach { aed ->
            val point = GeoPoint(aed.latitude, aed.longitude)
            if (point.latitude in bounds.latSouth..bounds.latNorth &&
                point.longitude in bounds.lonWest..bounds.lonEast
            ) { // wyświetlanie tylko markerów wewnątrz widocznego obszaru mapy
                val marker = Marker(map).apply {
                    position = point
                    title = aed.description ?: "AED"
                    subDescription = if (aed.verified) "Zweryfikowany" else "Niezweryfikowany"
                    setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM)

                    setOnMarkerClickListener { m, _ ->
                        lastOpenInfoWindow?.close()

                        val card = createInfoWindow(aed)
                        val info = object : InfoWindow(card, map) {
                            override fun onOpen(item: Any?) {
                                lastOpenInfoWindow = this
                            }

                            override fun onClose() {
                                if (lastOpenInfoWindow == this) lastOpenInfoWindow = null
                            }
                        }

                        info.open(m, m.position, 0, -m.icon.intrinsicHeight)
                        true
                    }
                }
                map.overlays.add(marker)
            }
        }

        map.invalidate()
    }

    Box(modifier = Modifier.fillMaxSize()) {

        if (showEnableLocationDialog) {
            AlertDialog(
                onDismissRequest = {},
                title = { Text("Udostępnij lokalizację") },
                text = { Text("Aby sprawdzić swoją pozycję na mapie, włącz lokalizację w ustawieniach.") },
                confirmButton = {
                    androidx.compose.material3.Button(onClick = {
                        openLocationSettings()
                        showEnableLocationDialog = false
                    }) { Text("OK") }
                },
                dismissButton = {
                    androidx.compose.material3.Button(onClick = {
                        showEnableLocationDialog = false
                    }) { Text("Anuluj") }
                }
            )
        }

        AndroidView(
            factory = { mapView },
            modifier = Modifier.fillMaxSize(),
            update = { map ->
                if (hasLocationPermission && isLocationEnabled(context)) {
                    val locationProvider = GpsMyLocationProvider(context)
                    val myLocationOverlay = MyLocationNewOverlay(locationProvider, map)
                    myLocationOverlay.enableMyLocation()
                    myLocationOverlay.runOnFirstFix {
                        if (!isCenteredOnUser) {
                            val loc = myLocationOverlay.myLocation
                            if (loc != null) {
                                Handler(Looper.getMainLooper()).post {
                                    map.controller.animateTo(loc)
                                    isCenteredOnUser = true
                                }
                            }
                        }
                    }
                    if (!map.overlays.contains(myLocationOverlay)) {
                        map.overlays.add(myLocationOverlay)
                    }
                }

                updateMarkers(map, aeds)

                map.setMapListener(object : MapListener {
                    override fun onScroll(event: ScrollEvent?): Boolean {
                        updateMarkers(map, aeds)
                        return true
                    }

                    override fun onZoom(event: ZoomEvent?): Boolean {
                        updateMarkers(map, aeds)
                        return true
                    }
                })
            }
        )

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
