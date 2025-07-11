// Variables globales del reproductor
let currentAudio = null;
let isPlaying = false;
let currentPlaylist = [];
let currentTrackIndex = 0;
let isPlaylistMode = false;

// Función para reproducir una sola canción
function playMusic(id, titulo, artista, audioUrl, event) {
    // Detener la propagación si el evento está disponible
    if (event) {
        event.stopPropagation();
        event.preventDefault();
    }

    console.log(`🎵 Reproduciendo canción individual: ${titulo} - ${artista}`);
    isPlaylistMode = false;
    currentPlaylist = [{ id, titulo, artista, audioUrl }];
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir playlist completa
function playPlaylist(playlistMusics) {
    console.log(`🎵 Reproduciendo playlist completa: ${playlistMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = playlistMusics;
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir album completo
function playAlbum(albumMusics) {
    console.log(`🎵 Reproduciendo album completo: ${albumMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = albumMusics;
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir playlist en modo aleatorio
function shufflePlaylist(playlistMusics) {
    console.log(`🎵 Reproduciendo playlist aleatoriamente: ${playlistMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = shuffleArray([...playlistMusics]);
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir desde un track específico en una playlist
function playFromTrack(playlistMusics, startIndex = 0) {
    console.log(`🎵 Reproduciendo desde track ${startIndex + 1} de ${playlistMusics.length}`);
    isPlaylistMode = true;
    currentPlaylist = playlistMusics;
    currentTrackIndex = startIndex;
    playTrack();
}

// Función principal para reproducir el track actual
function playTrack() {
    if (currentTrackIndex >= currentPlaylist.length) {
        console.log('📋 Final de la playlist alcanzado');
        onPlaylistEnd();
        return;
    }

    const track = currentPlaylist[currentTrackIndex];
    const player = document.getElementById('musicPlayer');
    const audioPlayer = document.getElementById('audioPlayer');
    const songInfo = document.getElementById('currentSong');
    const playPauseBtn = document.getElementById('playPauseBtn');

    console.log(`🎵 Cargando: ${track.titulo} - ${track.artista}`);

    // Mostrar reproductor si está oculto
    if (player) {
        player.style.display = 'block';
    }

    // Actualizar información de la canción
    if (songInfo) {
        songInfo.textContent = `${track.titulo} - ${track.artista}`;
        songInfo.title = `${track.titulo} - ${track.artista}`; // Tooltip para nombres largos
    }

    // Cargar nueva canción
    if (audioPlayer) {
        audioPlayer.src = track.audioUrl;
        audioPlayer.load();

        // Intentar reproducir
        audioPlayer.play().then(() => {
            isPlaying = true;
            if (playPauseBtn) {
                playPauseBtn.textContent = '⏸️';
                playPauseBtn.title = 'Pausar';
            }
            console.log(`✅ Reproduciendo: ${track.titulo}`);
        }).catch(error => {
            console.error(`❌ Error reproduciendo ${track.titulo}:`, error);
            // Si falla, mostrar error y pasar a la siguiente canción
            showErrorMessage(`No se pudo reproducir: ${track.titulo}`);
            nextTrack();
        });
    }
}

// Función para pasar a la siguiente canción
function nextTrack() {
    if (isPlaylistMode && currentTrackIndex < currentPlaylist.length - 1) {
        currentTrackIndex++;
        console.log(`⏭️ Siguiente: Track ${currentTrackIndex + 1} de ${currentPlaylist.length}`);
        playTrack();
    } else {
        console.log('📋 No hay más canciones - fin de playlist');
        onPlaylistEnd();
    }
}

// Función para ir a la canción anterior
function previousTrack() {
    if (isPlaylistMode && currentTrackIndex > 0) {
        currentTrackIndex--;
        console.log(`⏮️ Anterior: Track ${currentTrackIndex + 1} de ${currentPlaylist.length}`);
        playTrack();
    } else {
        console.log('📋 Ya estás en la primera canción');
        // Opcionalmente reiniciar la canción actual
        const audioPlayer = document.getElementById('audioPlayer');
        if (audioPlayer) {
            audioPlayer.currentTime = 0;
        }
    }
}

// Función cuando termina la playlist
function onPlaylistEnd() {
    const playPauseBtn = document.getElementById('playPauseBtn');
    const songInfo = document.getElementById('currentSong');

    if (playPauseBtn) {
        playPauseBtn.textContent = '▶️';
        playPauseBtn.title = 'Reproducir';
    }

    isPlaying = false;

    if (isPlaylistMode && songInfo) {
        songInfo.textContent = 'Playlist finalizada';
        console.log('🏁 Playlist finalizada');
    }
}

// Función para alternar play/pause
function togglePlayPause() {
    const audioPlayer = document.getElementById('audioPlayer');
    const playPauseBtn = document.getElementById('playPauseBtn');

    if (!audioPlayer) {
        console.error('❌ Reproductor de audio no encontrado');
        return;
    }

    if (isPlaying) {
        audioPlayer.pause();
        if (playPauseBtn) {
            playPauseBtn.textContent = '▶️';
            playPauseBtn.title = 'Reproducir';
        }
        isPlaying = false;
        console.log('⏸️ Música pausada');
    } else {
        audioPlayer.play().then(() => {
            if (playPauseBtn) {
                playPauseBtn.textContent = '⏸️';
                playPauseBtn.title = 'Pausar';
            }
            isPlaying = true;
            console.log('▶️ Música reanudada');
        }).catch(error => {
            console.error('❌ Error al reanudar:', error);
            showErrorMessage('Error al reproducir la canción');
            nextTrack();
        });
    }
}

// Función para detener completamente la música
function stopMusic() {
    const audioPlayer = document.getElementById('audioPlayer');
    const playPauseBtn = document.getElementById('playPauseBtn');
    const songInfo = document.getElementById('currentSong');
    const progressBar = document.getElementById('progressBar');

    if (audioPlayer) {
        audioPlayer.pause();
        audioPlayer.currentTime = 0;
    }

    if (playPauseBtn) {
        playPauseBtn.textContent = '▶️';
        playPauseBtn.title = 'Reproducir';
    }

    if (songInfo) {
        songInfo.textContent = 'Ninguna canción seleccionada';
    }

    if (progressBar) {
        progressBar.value = 0;
    }

    isPlaying = false;
    isPlaylistMode = false;
    currentPlaylist = [];
    currentTrackIndex = 0;

    console.log('⏹️ Reproducción detenida');
}

// Función para actualizar la barra de progreso
function updateProgress() {
    const audioPlayer = document.getElementById('audioPlayer');
    const progressBar = document.getElementById('progressBar');
    const currentTime = document.getElementById('currentTime');
    const duration = document.getElementById('duration');

    if (audioPlayer && audioPlayer.duration && !isNaN(audioPlayer.duration)) {
        const progress = (audioPlayer.currentTime / audioPlayer.duration) * 100;

        if (progressBar) {
            progressBar.value = progress;
        }

        if (currentTime) {
            currentTime.textContent = formatTime(audioPlayer.currentTime);
        }

        if (duration) {
            duration.textContent = formatTime(audioPlayer.duration);
        }
    }
}

// Función para buscar a una posición específica
function seekTo() {
    const audioPlayer = document.getElementById('audioPlayer');
    const progressBar = document.getElementById('progressBar');

    if (audioPlayer && audioPlayer.duration && progressBar) {
        const seekTime = (progressBar.value / 100) * audioPlayer.duration;
        audioPlayer.currentTime = seekTime;
        console.log(`🔍 Buscando a: ${formatTime(seekTime)}`);
    }
}

// Función para formatear tiempo
function formatTime(seconds) {
    if (isNaN(seconds) || seconds < 0) {
        return '0:00';
    }

    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

// Función cuando termina una canción
function onSongEnd() {
    console.log('🎵 Canción terminada');

    if (isPlaylistMode) {
        nextTrack();
    } else {
        const playPauseBtn = document.getElementById('playPauseBtn');
        const songInfo = document.getElementById('currentSong');

        if (playPauseBtn) {
            playPauseBtn.textContent = '▶️';
            playPauseBtn.title = 'Reproducir';
        }

        isPlaying = false;

        if (songInfo) {
            songInfo.textContent = 'Canción finalizada';
        }

        console.log('🏁 Reproducción individual terminada');
    }
}

// Función helper para mezclar arrays (Fisher-Yates shuffle)
function shuffleArray(array) {
    const shuffled = [...array]; // Crear copia para no modificar el original
    for (let i = shuffled.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
}

// Función para mostrar mensajes de error (opcional)
function showErrorMessage(message) {
    console.error(`❌ ${message}`);

    // Si tienes un sistema de notificaciones, úsalo aquí
    if (typeof TempData !== 'undefined') {
        TempData["Error"] = message;
    }

    // O mostrar alert simple (opcional, puedes comentar esta línea)
    // alert(message);
}

// Función para obtener información del track actual
function getCurrentTrackInfo() {
    if (currentPlaylist.length > 0 && currentTrackIndex >= 0 && currentTrackIndex < currentPlaylist.length) {
        return {
            track: currentPlaylist[currentTrackIndex],
            index: currentTrackIndex,
            total: currentPlaylist.length,
            isPlaying: isPlaying,
            isPlaylistMode: isPlaylistMode
        };
    }
    return null;
}

// Función para cambiar el volumen (si quieres agregar control de volumen)
function setVolume(volume) {
    const audioPlayer = document.getElementById('audioPlayer');
    if (audioPlayer) {
        audioPlayer.volume = Math.max(0, Math.min(1, volume)); // Entre 0 y 1
        console.log(`🔊 Volumen ajustado a: ${Math.round(volume * 100)}%`);
    }
}

// Inicialización cuando se carga el DOM
document.addEventListener('DOMContentLoaded', function () {
    console.log('🎵 Reproductor SonikoMusic inicializado');

    // Configurar volumen inicial
    const audioPlayer = document.getElementById('audioPlayer');
    if (audioPlayer) {
        audioPlayer.volume = 0.7; // 70% de volumen por defecto
    }
});

// Exportar funciones si se usa como módulo (opcional)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        playMusic,
        playPlaylist,
        playAlbum,
        shufflePlaylist,
        playFromTrack,
        togglePlayPause,
        stopMusic,
        nextTrack,
        previousTrack,
        getCurrentTrackInfo,
        setVolume
    };
}

