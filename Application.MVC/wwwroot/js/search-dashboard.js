
let currentMusicId = 0;

function showAddToPlaylistModal(musicId, songTitle) {
    currentMusicId = musicId;
    document.getElementById('songTitle').textContent = songTitle;
    loadUserPlaylists();
    new bootstrap.Modal(document.getElementById('addToPlaylistModal')).show();
}

function loadUserPlaylists() {
    const container = document.getElementById('playlistsList');
    container.innerHTML = '<div class="text-center"><i class="fas fa-spinner fa-spin"></i> Cargando playlists...</div>';

    fetch('/Playlists/GetUserPlaylists')
        .then(response => response.json())
        .then(data => {
            container.innerHTML = '';

            if (data.error) {
                container.innerHTML = '<p class="text-danger">Error: ' + data.error + '</p>';
                return;
            }

            if (data.length === 0) {
                container.innerHTML = `
                        <div class="text-center p-3">
                            <i class="fas fa-list fa-2x text-muted mb-2"></i>
                            <p class="text-muted">No tienes playlists aún.</p>
                            <p class="text-muted">Crea tu primera playlist para agregar música.</p>
                        </div>
                    `;
                return;
            }

            data.forEach(playlist => {
                const playlistItem = document.createElement('div');
                playlistItem.className = 'd-flex justify-content-between align-items-center p-3 border rounded mb-2 playlist-item';
                playlistItem.innerHTML = `
                        <div class="playlist-info">
                            <div class="d-flex align-items-center">
                                <i class="fas fa-list text-success me-2"></i>
                                <div>
                                    <strong>${playlist.name}</strong>
                                    <div class="small text-muted">
                                        ${playlist.description || 'Sin descripción'} •
                                        ${playlist.songsCount} canción(es) •
                                        ${playlist.isPublic ? '🌐 Pública' : '🔒 Privada'}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <button class="btn btn-primary btn-sm" onclick="addToPlaylist(${playlist.id}, '${playlist.name}')">
                            <i class="fas fa-plus"></i> Agregar
                        </button>
                    `;
                container.appendChild(playlistItem);
            });
        })
        .catch(error => {
            console.error('Error loading playlists:', error);
            container.innerHTML = '<p class="text-danger">Error al cargar playlists. Intenta de nuevo.</p>';
        });
}

function addToPlaylist(playlistId, playlistName) {
    const button = event.target.closest('button');
    const originalContent = button.innerHTML;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Agregando...';
    button.disabled = true;

    const formData = new FormData();
    formData.append('playlistId', playlistId);
    formData.append('musicId', currentMusicId);

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    if (token) {
        formData.append('__RequestVerificationToken', token);
    }

    fetch('/PlaylistMusic/AddToPlaylist', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                button.innerHTML = '<i class="fas fa-check text-success"></i> ¡Agregada!';
                button.className = 'btn btn-success btn-sm';

                setTimeout(() => {
                    bootstrap.Modal.getInstance(document.getElementById('addToPlaylistModal')).hide();
                    showNotification(`Música agregada a "${playlistName}" exitosamente`, 'success');
                }, 1000);
            } else {
                button.innerHTML = originalContent;
                button.disabled = false;
                showNotification('Error al agregar música a la playlist', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            button.innerHTML = originalContent;
            button.disabled = false;
            showNotification('Error de conexión', 'error');
        });
}

function showNotification(message, type) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';

    const notification = document.createElement('div');
    notification.className = `alert ${alertClass} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
            <i class="${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

    document.body.appendChild(notification);

    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}
