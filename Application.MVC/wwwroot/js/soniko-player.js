// Variables globales del reproductor
let currentAudio = null;
let isPlaying = false;
let currentPlaylist = [];
let currentTrackIndex = 0;
let isPlaylistMode = false;

// ===== FUNCIONES DE PERSISTENCIA =====

// Guardar estado actual en localStorage
function savePlayerState() {
    const audioPlayer = document.getElementById('audioPlayer');

    if (currentPlaylist.length > 0 && currentTrackIndex >= 0) {
        const playerState = {
            currentPlaylist: currentPlaylist,
            currentTrackIndex: currentTrackIndex,
            isPlaylistMode: isPlaylistMode,
            currentTime: audioPlayer ? audioPlayer.currentTime : 0,
            isPlaying: isPlaying,
            volume: audioPlayer ? audioPlayer.volume : 0.7,
            timestamp: Date.now()
        };

        localStorage.setItem('sonikoPlayer', JSON.stringify(playerState));
        console.log('💾 Estado del reproductor guardado');
    }
}

// Restaurar estado desde localStorage
function loadPlayerState() {

    
    const isAuthenticated = document.querySelector('.navbar-nav .nav-link[href*="logout"]') !== null ||
        document.querySelector('form[action*="Logout"]') !== null ||
        document.body.classList.contains('authenticated');

    if (!isAuthenticated) {
        console.log('👤 Usuario no autenticado - no restaurar reproductor');
        localStorage.removeItem('sonikoPlayer'); // Limpiar cualquier estado previo
        return false;
    }
    try {
        const savedState = localStorage.getItem('sonikoPlayer');
        if (!savedState) return false;

        const playerState = JSON.parse(savedState);

        // Verificar que el estado no sea muy antiguo (más de 24 horas)
        const hoursSinceLastPlay = (Date.now() - playerState.timestamp) / (1000 * 60 * 60);
        if (hoursSinceLastPlay > 24) {
            localStorage.removeItem('sonikoPlayer');
            return false;
        }

        // Restaurar variables globales
        currentPlaylist = playerState.currentPlaylist || [];
        currentTrackIndex = playerState.currentTrackIndex || 0;
        isPlaylistMode = playerState.isPlaylistMode || false;

        // Verificar que tenemos datos válidos
        if (currentPlaylist.length === 0 || currentTrackIndex >= currentPlaylist.length) {
            return false;
        }

        const audioPlayer = document.getElementById('audioPlayer');
        const player = document.getElementById('musicPlayer');

        if (audioPlayer && player) {
            // Mostrar reproductor
            player.style.display = 'block';
            player.classList.remove('closed'); 
            showPlayer();

            // Configurar volumen
            audioPlayer.volume = playerState.volume || 0.7;

            // Cargar la canción actual
            const track = currentPlaylist[currentTrackIndex];
            updatePlayerUI(track);

            audioPlayer.src = track.audioUrl;
            audioPlayer.currentTime = playerState.currentTime || 0;
            setTimeout(() => {
                updateProgress(); // Actualizar barra de progreso después de cargar
            }, 100);


            // Restaurar estado de reproducción
            if (playerState.isPlaying) {


                audioPlayer.play().then(() => {
                    isPlaying = true;
                    updatePlayPauseButton(true);
                    console.log('🎵 Reproducción restaurada automáticamente');


                }).catch(error => {
                    console.log('⚠️ No se pudo reanudar automáticamente (política del navegador)');
                    isPlaying = false;
                    updatePlayPauseButton(false);
                });
            } else {
                isPlaying = false;
                updatePlayPauseButton(false);
            }


            setTimeout(() => {
                const player = document.getElementById('musicPlayer');
                if (player && currentPlaylist.length > 0) {
                    player.style.display = 'block';
                    player.classList.remove('closed');
                    updateProgress(); 
                }
            }, 200);

            console.log('Estado del reproductor restaurado');
            return true;
        }

    } catch (error) {
        console.error('❌ Error restaurando estado:', error);
        localStorage.removeItem('sonikoPlayer');
    }

    return false;
}

// Actualizar UI del reproductor
function updatePlayerUI(track) {
    const songInfo = document.getElementById('currentSong');
    if (songInfo) {
        songInfo.textContent = `${track.titulo} - ${track.artista}`;
        songInfo.title = `${track.titulo} - ${track.artista}`;
    }
}

// Actualizar botón play/pause
function updatePlayPauseButton(playing) {
    const playPauseBtn = document.getElementById('playPauseBtn');
    if (playPauseBtn) {
        playPauseBtn.textContent = playing ? '⏸️' : '▶️';
        playPauseBtn.title = playing ? 'Pausar' : 'Reproducir';
    }
}

// ===== MODIFICACIONES A FUNCIONES EXISTENTES =====

// Función para reproducir una sola canción (MODIFICADA)
function playMusic(id, titulo, artista, audioUrl, event) {
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

// Función para reproducir playlist completa (MODIFICADA)
function playPlaylist(playlistMusics) {
    console.log(`🎵 Reproduciendo playlist completa: ${playlistMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = playlistMusics;
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir album completo (MODIFICADA)
function playAlbum(albumMusics) {
    console.log(`🎵 Reproduciendo album completo: ${albumMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = albumMusics;
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir playlist en modo aleatorio (MODIFICADA)
function shufflePlaylist(playlistMusics) {
    console.log(`🎵 Reproduciendo playlist aleatoriamente: ${playlistMusics.length} canciones`);
    isPlaylistMode = true;
    currentPlaylist = shuffleArray([...playlistMusics]);
    currentTrackIndex = 0;
    playTrack();
}

// Función para reproducir desde un track específico (MODIFICADA)
function playFromTrack(playlistMusics, startIndex = 0) {
    console.log(`🎵 Reproduciendo desde track ${startIndex + 1} de ${playlistMusics.length}`);
    isPlaylistMode = true;
    currentPlaylist = playlistMusics;
    currentTrackIndex = startIndex;
    playTrack();
}

// Función principal para reproducir el track actual (MODIFICADA)
function playTrack() {
    if (currentTrackIndex >= currentPlaylist.length) {
        console.log('📋 Final de la playlist alcanzado');
        onPlaylistEnd();
        return;
    }

    const track = currentPlaylist[currentTrackIndex];
    const player = document.getElementById('musicPlayer');
    const audioPlayer = document.getElementById('audioPlayer');

    console.log(`🎵 Cargando: ${track.titulo} - ${track.artista}`);

    // Mostrar reproductor si está oculto
    if (player) {
        player.style.display = 'block';
        player.classList.remove('closed');
        showPlayer();
    }

    // Actualizar información de la canción
    updatePlayerUI(track);

    // Cargar nueva canción
    if (audioPlayer) {
        audioPlayer.src = track.audioUrl;
        audioPlayer.load();

        // Intentar reproducir
        audioPlayer.play().then(() => {
            isPlaying = true;
            updatePlayPauseButton(true);
            savePlayerState(); // GUARDAR ESTADO
            console.log(`✅ Reproduciendo: ${track.titulo}`);
        }).catch(error => {
            console.error(`❌ Error reproduciendo ${track.titulo}:`, error);
            showErrorMessage(`No se pudo reproducir: ${track.titulo}`);
            nextTrack();
        });
    }
}

// Función para alternar play/pause (MODIFICADA)
function togglePlayPause() {
    const audioPlayer = document.getElementById('audioPlayer');

    if (!audioPlayer) {
        console.error('❌ Reproductor de audio no encontrado');
        return;
    }

    if (isPlaying) {
        audioPlayer.pause();
        updatePlayPauseButton(false);
        isPlaying = false;
        savePlayerState(); // GUARDAR ESTADO
        console.log('⏸️ Música pausada');
    } else {
        audioPlayer.play().then(() => {
            updatePlayPauseButton(true);
            isPlaying = true;
            savePlayerState(); // GUARDAR ESTADO
            console.log('▶️ Música reanudada');
        }).catch(error => {
            console.error('❌ Error al reanudar:', error);
            showErrorMessage('Error al reproducir la canción');
            nextTrack();
        });
    }
}

// Función para pasar a la siguiente canción (MODIFICADA)
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

// Función para ir a la canción anterior (MODIFICADA)
function previousTrack() {
    if (isPlaylistMode && currentTrackIndex > 0) {
        currentTrackIndex--;
        console.log(`⏮️ Anterior: Track ${currentTrackIndex + 1} de ${currentPlaylist.length}`);
        playTrack();
    } else {
        console.log('📋 Ya estás en la primera canción');
        const audioPlayer = document.getElementById('audioPlayer');
        if (audioPlayer) {
            audioPlayer.currentTime = 0;
            savePlayerState(); // GUARDAR ESTADO
        }
    }
}

// Función para detener completamente la música (MODIFICADA)
function stopMusic() {
    const audioPlayer = document.getElementById('audioPlayer');
    const songInfo = document.getElementById('currentSong');
    const progressBar = document.getElementById('progressBar');

    if (audioPlayer) {
        audioPlayer.pause();
        audioPlayer.currentTime = 0;
    }

    updatePlayPauseButton(false);

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

    // LIMPIAR ESTADO GUARDADO
    localStorage.removeItem('sonikoPlayer');
    console.log('⏹️ Reproducción detenida y estado limpiado');
}

// Función para actualizar la barra de progreso (MODIFICADA)
function updateProgress() {
    const audioPlayer = document.getElementById('audioPlayer');
    const progressBar = document.getElementById('progressBar');
    const currentTime = document.getElementById('currentTime');
    const duration = document.getElementById('duration');

    if (audioPlayer && audioPlayer.duration && !isNaN(audioPlayer.duration)) {
        const progress = (audioPlayer.currentTime / audioPlayer.duration) * 100;

        if (progressBar) {
            progressBar.value = progress;
            progressBar.style.setProperty('--progress', progress + '%');

        }

        if (currentTime) {
            currentTime.textContent = formatTime(audioPlayer.currentTime);
        }

        if (duration) {
            duration.textContent = formatTime(audioPlayer.duration);
        }

        // GUARDAR PROGRESO CADA 5 SEGUNDOS
        if (isPlaying && Math.floor(audioPlayer.currentTime) % 5 === 0) {
            savePlayerState();
        }
    }
}

// Función para buscar a una posición específica (MODIFICADA)
function seekTo() {
    const audioPlayer = document.getElementById('audioPlayer');
    const progressBar = document.getElementById('progressBar');

    if (audioPlayer && audioPlayer.duration && progressBar) {
        const seekTime = (progressBar.value / 100) * audioPlayer.duration;
        audioPlayer.currentTime = seekTime;
        savePlayerState(); // GUARDAR ESTADO
        console.log(`🔍 Buscando a: ${formatTime(seekTime)}`);
    }
}

// Función cuando termina una canción (MODIFICADA)
function onSongEnd() {
    console.log('🎵 Canción terminada');

    if (isPlaylistMode) {
        nextTrack();
    } else {
        updatePlayPauseButton(false);
        isPlaying = false;
        savePlayerState(); // GUARDAR ESTADO

        const songInfo = document.getElementById('currentSong');
        if (songInfo) {
            songInfo.textContent = 'Canción finalizada';
        }

        console.log('🏁 Reproducción individual terminada');
    }
}

// ===== FUNCIONES EXISTENTES SIN CAMBIOS =====

function formatTime(seconds) {
    if (isNaN(seconds) || seconds < 0) {
        return '0:00';
    }

    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

function onPlaylistEnd() {
    updatePlayPauseButton(false);
    isPlaying = false;
    savePlayerState(); // GUARDAR ESTADO

    const songInfo = document.getElementById('currentSong');
    if (isPlaylistMode && songInfo) {
        songInfo.textContent = 'Playlist finalizada';
        console.log('🏁 Playlist finalizada');
    }
}

function shuffleArray(array) {
    const shuffled = [...array];
    for (let i = shuffled.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
}

function showErrorMessage(message) {
    console.error(`❌ ${message}`);
}

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

function setVolume(volume) {
    const audioPlayer = document.getElementById('audioPlayer');
    if (audioPlayer) {
        audioPlayer.volume = Math.max(0, Math.min(1, volume));
        savePlayerState(); // GUARDAR ESTADO
        console.log(`🔊 Volumen ajustado a: ${Math.round(volume * 100)}%`);
    }
}

// ===== INICIALIZACIÓN (MODIFICADA) =====
document.addEventListener('DOMContentLoaded', function () {
    console.log('🎵 Reproductor SonikoMusic inicializando...');

    // Configurar volumen inicial
    const audioPlayer = document.getElementById('audioPlayer');
    if (audioPlayer) {
        audioPlayer.volume = 0.7;
    }

    // INTENTAR RESTAURAR ESTADO PREVIO
    setTimeout(() => {
        const restored = loadPlayerState();
        if (restored) {
            console.log('✅ Estado anterior restaurado exitosamente');
        } else {
            console.log('ℹ️ No hay estado anterior para restaurar');
        }
    }, 500); // Pequeño delay para asegurar que el DOM está listo

    console.log('🎵 Reproductor SonikoMusic inicializado');
});

// ===== LIMPIAR AL CERRAR VENTANA =====
window.addEventListener('beforeunload', function () {
    if (isPlaying) {
        savePlayerState();
        console.log('💾 Estado guardado antes de cerrar');
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
        setVolume,
        savePlayerState,
        loadPlayerState
    };
}

















// ===== EVENTOS DEL REPRODUCTOR =====
function closePlayer() {
    const player = document.getElementById('musicPlayer');
    if (player) {
        // Pausar música si está reproduciéndose
        if (isPlaying) {
            togglePlayPause();
        }

        // Ocultar reproductor con animación
        player.classList.add('closed');

        console.log('🔇 Reproductor cerrado manualmente');
    }
}

// Función mejorada para actualizar el botón play/pause
function updatePlayPauseButton(playing) {
    const playPauseBtn = document.getElementById('playPauseBtn');
    if (playPauseBtn) {
        const icon = playPauseBtn.querySelector('i');
        if (icon) {
            icon.className = playing ? 'fas fa-pause' : 'fas fa-play';
        }

        playPauseBtn.title = playing ? 'Pausar' : 'Reproducir';

        // Agregar clase para cambiar color
        if (playing) {
            playPauseBtn.classList.add('playing');
        } else {
            playPauseBtn.classList.remove('playing');
        }
    }
}

// Función mejorada para actualizar información de la canción
function updatePlayerUI(track) {
    const songTitle = document.querySelector('.song-title');
    const songArtist = document.querySelector('.song-artist');

    if (songTitle && songArtist) {
        songTitle.textContent = track.titulo;
        songTitle.title = track.titulo; // Tooltip

        songArtist.textContent = track.artista;
        songArtist.title = track.artista; // Tooltip
    }
}

// Mostrar reproductor con animación mejorada
function showPlayer() {
    const player = document.getElementById('musicPlayer');
    if (player) {
        player.classList.remove('closed');
        player.style.display = 'block';
    }
}

// Modificar la función playTrack para usar las nuevas funciones
const originalPlayTrack = window.playTrack;
if (originalPlayTrack) {
    window.playTrack = function () {
        showPlayer(); // Mostrar reproductor antes de reproducir
        return originalPlayTrack.apply(this, arguments);
    };
}

// Mejorar el formateo de tiempo
function formatTime(seconds) {
    if (isNaN(seconds) || seconds < 0) {
        return '0:00';
    }

    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);

    if (mins >= 60) {
        const hours = Math.floor(mins / 60);
        const remainingMins = mins % 60;
        return `${hours}:${remainingMins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }

    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

// Inicialización mejorada
document.addEventListener('DOMContentLoaded', function () {
    // Ocultar reproductor inicialmente
    const player = document.getElementById('musicPlayer');
    if (player) {
        player.classList.add('closed');
    }

    console.log('🎵 Reproductor mejorado inicializado');
});
