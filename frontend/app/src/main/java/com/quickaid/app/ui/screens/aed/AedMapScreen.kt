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
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.viewinterop.AndroidView
import androidx.core.app.ActivityCompat
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.HomeButton
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
    sessionViewModel: SessionViewModel = hiltViewModel(),
    onEditAed: (id: Int?, externalId: Long?) -> Unit = { _, _ -> }
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
            controller.setCenter(GeoPoint(52.2297, 21.0122)) // fallback Warsaw
            setOnTouchListener { _, _ ->
                lastOpenInfoWindow?.close()
                false
            }
        }
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

                val existingMarkers = map.overlays.filterIsInstance<Marker>()
                val existingPositions = existingMarkers.map { it.position to it.title }.toSet()

                aeds.forEach { aed ->
                    val point = GeoPoint(aed.latitude, aed.longitude)
                    if ((point to (aed.description ?: "AED")) !in existingPositions) {
                        val marker = Marker(map).apply {
                            position = point
                            title = aed.description ?: "AED"
                            subDescription = if (aed.verified) "Zweryfikowany" else "Niezweryfikowany"
                            setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM)

                            val card = CardView(context).apply {
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
                                        text = title
                                        setTextColor(Color.BLACK)
                                        setTextSize(TypedValue.COMPLEX_UNIT_SP, 16f)
                                    }
                                    val descView = TextView(context).apply {
                                        text = subDescription
                                        setTextColor(Color.DKGRAY)
                                        setTextSize(TypedValue.COMPLEX_UNIT_SP, 14f)
                                    }
                                    addView(titleView)
                                    addView(descView)
                                    if (userRole != UserRole.ANON) {
                                        val editBtn = Button(context).apply {
                                            text = "Edytuj"
                                            setOnClickListener { onEditAed(aed.id, aed.externalId) }
                                        }
                                        addView(editBtn)
                                    }
                                })
                            }

                            infoWindow = object : InfoWindow(card, map) {
                                override fun onOpen(item: Any?) {
                                    lastOpenInfoWindow?.close()
                                    lastOpenInfoWindow = this
                                }

                                override fun onClose() {
                                    if (lastOpenInfoWindow == this) lastOpenInfoWindow = null
                                }
                            }
                        }
                        map.overlays.add(marker)
                    }
                }

                map.invalidate()
            }
        )

        HomeButton(onClick = {
            navController.navigate("home") {
                popUpTo(navController.graph.startDestinationId) { inclusive = true }
            }
        },
            modifier = Modifier
                .align(Alignment.TopEnd)
                .padding(AppSizes.medium)
                .size(AppSizes.extraLarge)
        )
    }
}
