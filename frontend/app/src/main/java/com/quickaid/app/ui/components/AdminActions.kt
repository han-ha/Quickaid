package com.quickaid.app.ui.components

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import com.quickaid.app.ui.theme.AppSizes

@Composable
fun AdminActions(
    onEdit: () -> Unit,
    onDelete: () -> Unit,
    modifier: Modifier = Modifier
) {
    Row(
        modifier = modifier,
        horizontalArrangement = Arrangement.spacedBy(AppSizes.small)
    ) {
        SmallButton(
            onClick = onEdit,
            content = "Edytuj"
        )
        SmallButton(
            onClick = onDelete,
            content = "Usuń",
            buttonColor = MaterialTheme.colorScheme.error
        )
    }
}
