let notificationDropdownOpen = false;

function toggleNotifications() {
    const dropdown = document.getElementById('notificationDropdown');
    notificationDropdownOpen = !notificationDropdownOpen;

    if (notificationDropdownOpen) {
        dropdown.classList.add('show');
        loadRecentNotifications();
    } else {
        dropdown.classList.remove('show');
    }
}

// Cerrar dropdown al hacer click fuera
document.addEventListener('click', function (event) {
    const bellContainer = document.querySelector('.notification-bell-container');
    if (!bellContainer.contains(event.target)) {
        document.getElementById('notificationDropdown').classList.remove('show');
        notificationDropdownOpen = false;
    }
});

function loadRecentNotifications() {
    fetch('/Notifications/GetRecent') // 
        .then(response => response.json())
        .then(notifications => {
            const container = document.getElementById('notificationsList');

            if (notifications.length === 0) {
                container.innerHTML = `
                    <div style="padding: 20px; text-align: center; color: #666;">
                        No tienes notificaciones recientes
                    </div>
                `;
                return;
            }

            container.innerHTML = notifications.map(notification => `
                <div class="dropdown-notification ${notification.isRead ? '' : 'unread'}" 
                     onclick="handleNotificationClick(${notification.id}, '${notification.actionUrl}')">
                    <div class="dropdown-notification-title">${notification.title}</div>
                    <div class="dropdown-notification-message">${notification.message}</div>
                    <div class="dropdown-notification-time">${notification.createdDate}</div>
                </div>
            `).join('');
        })
        .catch(error => {
            console.error('Error cargando notificaciones:', error);
            document.getElementById('notificationsList').innerHTML = `
                <div style="padding: 20px; text-align: center; color: #666;">
                    Error cargando notificaciones
                </div>
            `;
        });
}

function handleNotificationClick(notificationId, actionUrl) {
    // Marcar como leída
    fetch('/Notifications/MarkAsRead', { // ✅ URL absoluta
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `id=${notificationId}&__RequestVerificationToken=${document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''}`
    });

    // Ir a la URL de acción si existe
    if (actionUrl && actionUrl !== 'null' && actionUrl !== '') {
        window.location.href = actionUrl;
    }

    // Cerrar dropdown
    document.getElementById('notificationDropdown').classList.remove('show');
    notificationDropdownOpen = false;

    // Actualizar contador
    updateNotificationCount();
}

function updateNotificationCount() {
    fetch('/Notifications/GetUnreadCount') // ✅ URL absoluta
        .then(response => response.json())
        .then(data => {
            const badge = document.getElementById('notificationCount');
            if (data.count > 0) {
                badge.textContent = data.count > 99 ? '99+' : data.count;
                badge.style.display = 'flex';
            } else {
                badge.style.display = 'none';
            }
        })
        .catch(error => {
            console.error('Error actualizando contador:', error);
        });
}

// Actualizar contador al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    updateNotificationCount();

    // Actualizar cada 30 segundos
    setInterval(updateNotificationCount, 30000);
});