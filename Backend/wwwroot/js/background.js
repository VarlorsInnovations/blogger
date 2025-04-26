
    const canvas = document.getElementById('networkCanvas');
    const ctx = canvas.getContext('2d');

    // Canvas auf Fenstergröße einstellen
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    // Parameter für das Netzwerk
    const particleCount = 120;
    const baseMaxDistance = 170;
    let maxDistance = baseMaxDistance;
    const particles = [];
    let mouseX = null;
    let mouseY = null;
    let hue = 0;
    let pulsePhase = 0;
    let globalAlpha = 0.5;
    let flowFieldTime = 0;

    // Farbpalette für Neuronen (leicht leuchtende Farben)
    const colors = [
    '#FF5E5B', // Rot
    '#D8D8F6', // Hellblau
    '#39E5B6', // Türkis
    '#FFED66', // Gelb
    '#00B2CA', // Blau
    '#FF9505', // Orange
    '#7B4B94', // Lila
    '#50E3C2', // Mint
    '#F8A3EB', // Rosa
    ];

    // Zufällige Farbe aus der Palette wählen
    function getRandomColor() {
    return colors[Math.floor(Math.random() * colors.length)];
}

    // Perlin Noise Funktion für organische Bewegungen (Vereinfachte Version)
    function noise(x, y) {
    return Math.sin(x * 0.01) * Math.cos(y * 0.01) * 2;
}

    // Partikel-Klasse mit erweiterten Funktionen
    class Particle {
    constructor() {
    this.reset();
    this.life = Math.random() * 0.5 + 0.5; // Lebensdauer für Fading-Effekt
    this.baseSize = this.size;
    this.active = true;
    this.activationDelay = Math.random() * 1000;
    this.activationTimer = 0;
    this.angularSpeed = (Math.random() - 0.5) * 0.02; // Für Rotationseffekt
    this.angle = Math.random() * Math.PI * 2;
    this.brightness = Math.random() * 20 + 80; // Helligkeit in Prozent
    this.glowSize = Math.random() * 10 + 5; // Größe des Leuchteffekts
}

    reset() {
        // Gleichmäßigere Verteilung mit Gitter + Zufall
        const gridSize = Math.sqrt(particleCount);
        const cellWidth = canvas.width / gridSize;
        const cellHeight = canvas.height / gridSize;

        // Finde eine Gitterzelle
        const cellX = Math.floor(Math.random() * gridSize);
        const cellY = Math.floor(Math.random() * gridSize);

        // Position innerhalb der Zelle mit etwas Zufall
        this.x = (cellX + 0.2 + Math.random() * 0.6) * cellWidth;
        this.y = (cellY + 0.2 + Math.random() * 0.6) * cellHeight;

        // Rest wie vorher
        this.size = Math.random() * 2.5 + 1;
        this.speedX = (Math.random() - 0.5) * 0.8;
        this.speedY = (Math.random() - 0.5) * 0.8;
        this.color = getRandomColor();
        this.pulseOffset = Math.random() * Math.PI * 2;
        this.originalColor = this.color;
}

    // Partikel aktualisieren mit komplexerer Logik
    update() {
    // Aktivierungsverzögerung
    this.activationTimer++;
    if (this.activationTimer < this.activationDelay) {
        return;
    }

        for (let i = 0; i < particles.length; i++) {
            if (particles[i] !== this) {
                const dx = this.x - particles[i].x;
                const dy = this.y - particles[i].y;
                const distance = Math.sqrt(dx * dx + dy * dy);

                if (distance < 40) { // Abstoßungsradius
                    const force = 0.02 / (distance + 0.1);
                    this.speedX += dx * force;
                    this.speedY += dy * force;
                }
            }
        }

        // Flow Field Einfluss für organische Bewegung
    const noiseValue = noise(this.x + flowFieldTime, this.y + flowFieldTime);
    this.speedX += Math.cos(noiseValue) * 0.005;
    this.speedY += Math.sin(noiseValue) * 0.005;

    // Geschwindigkeit begrenzen
    const speed = Math.sqrt(this.speedX * this.speedX + this.speedY * this.speedY);
    if (speed > 1.5) {
    this.speedX = (this.speedX / speed) * 1.5;
    this.speedY = (this.speedY / speed) * 1.5;
    }

    // Bewegung mit träger Beschleunigung
    this.x += this.speedX;
    this.y += this.speedY;

    // Rotation um einen imaginären Mittelpunkt
    this.angle += this.angularSpeed;
    
    // Mausinteraktion - Anziehung zum Mauszeiger
    if (mouseX !== null && mouseY !== null) {
    const dx = mouseX - this.x;
    const dy = mouseY - this.y;
    const distance = Math.sqrt(dx * dx + dy * dy);
    const maxForceDistance = 150;

    if (distance < maxForceDistance) {
    const force = (1 - distance / maxForceDistance) * 0.03;
    this.speedX += dx * force / distance;
    this.speedY += dy * force / distance;

    // Partikel in Mausnähe leuchten stärker
    this.brightness = 100 + (1 - distance / maxForceDistance) * 50;
} else {
    this.brightness = Math.max(80, this.brightness - 0.5);
}
}

    // Am Rand umkehren mit weichen Übergängen
    const margin = 50;
    if (this.x < margin) {
    this.speedX += 0.05;
} else if (this.x > canvas.width - margin) {
    this.speedX -= 0.05;
}

    if (this.y < margin) {
    this.speedY += 0.05;
} else if (this.y > canvas.height - margin) {
    this.speedY -= 0.05;
}

    // Komplett außerhalb? Zurücksetzen
    if (this.x < -50 || this.x > canvas.width + 50 ||
    this.y < -50 || this.y > canvas.height + 50) {
    this.reset();
}

    // Pulsieren der Größe
    const pulse = Math.sin(pulsePhase + this.pulseOffset) * 0.5 + 1;
    this.size = this.baseSize * pulse;

    // Gelegentliche Farbänderung
    if (Math.random() < 0.005) {
    this.color = getRandomColor();
}

    // Alterung und Neugeburt
    this.life -= 0.0003;
    if (this.life <= 0) {
    this.reset();
    this.life = Math.random() * 0.5 + 0.5;
}
}

    // Erweitertes Zeichnen mit Leuchteffekten
    draw() {
    // Nur zeichnen, wenn aktiv
    if (this.activationTimer < this.activationDelay) {
    return;
}

    // Glow-Effekt
    const glow = ctx.createRadialGradient(
    this.x, this.y, 0,
    this.x, this.y, this.glowSize
    );

    // Farbe für den Glow-Effekt extrahieren
    let r = 0, g = 0, b = 0;
    if (this.color.startsWith('#')) {
    const hex = this.color.substring(1);
    r = parseInt(hex.substring(0, 2), 16);
    g = parseInt(hex.substring(2, 4), 16);
    b = parseInt(hex.substring(4, 6), 16);
}

    // Multiplikator für die Helligkeit
    const brightnessFactor = this.brightness / 100;

    glow.addColorStop(0, `rgba(${r * brightnessFactor}, ${g * brightnessFactor}, ${b * brightnessFactor}, ${0.8 * this.life})`);
    glow.addColorStop(1, `rgba(${r}, ${g}, ${b}, 0)`);

    ctx.beginPath();
    ctx.arc(this.x, this.y, this.glowSize, 0, Math.PI * 2);
    ctx.fillStyle = glow;
    ctx.fill();

    // Hauptpartikel
    ctx.beginPath();
    ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
    ctx.fillStyle = this.color;
    ctx.globalAlpha = this.life * 0.9;
    ctx.fill();
    ctx.globalAlpha = 1;

    // Weißer Kernpunkt für zusätzlichen Glanz
    ctx.beginPath();
    ctx.arc(this.x, this.y, this.size * 0.5, 0, Math.PI * 2);
    ctx.fillStyle = `rgba(255, 255, 255, ${this.life * 0.7})`;
    ctx.fill();
}
}

    // Partikel erstellen
    for (let i = 0; i < particleCount; i++) {
    particles.push(new Particle());
}

    // Verbindungen zwischen Partikeln zeichnen mit verbesserten Effekten
    function drawConnections() {
    for (let i = 0; i < particles.length; i++) {
    for (let j = i + 1; j < particles.length; j++) {
    const dx = particles[i].x - particles[j].x;
    const dy = particles[i].y - particles[j].y;
    const distance = Math.sqrt(dx * dx + dy * dy);

    if (distance < maxDistance) {
    // Transparenz basierend auf Entfernung
    const opacity = 1 - (distance / maxDistance);

    // Farbübergang zwischen den Partikeln für die Verbindung
    const gradient = ctx.createLinearGradient(
    particles[i].x, particles[i].y,
    particles[j].x, particles[j].y
    );

    gradient.addColorStop(0, particles[i].color.replace(')', `, ${opacity * globalAlpha})`).replace('rgb', 'rgba'));
    gradient.addColorStop(1, particles[j].color.replace(')', `, ${opacity * globalAlpha})`).replace('rgb', 'rgba'));

    ctx.beginPath();
    ctx.moveTo(particles[i].x, particles[i].y);
    ctx.lineTo(particles[j].x, particles[j].y);

    // Linienbreite basierend auf Entfernung
    const lineWidth = ((1 - distance / maxDistance) * 2) *
    Math.min(particles[i].life, particles[j].life);
    ctx.lineWidth = lineWidth;

    ctx.strokeStyle = gradient;
    ctx.stroke();

    // Energiepulse auf den Verbindungen (kleine bewegende Punkte)
    if (distance < maxDistance * 0.8 && Math.random() < 0.03) {
    const pulsePos = Math.random();
    const pulseX = particles[i].x * (1 - pulsePos) + particles[j].x * pulsePos;
    const pulseY = particles[i].y * (1 - pulsePos) + particles[j].y * pulsePos;

    ctx.beginPath();
    ctx.arc(pulseX, pulseY, lineWidth * 2, 0, Math.PI * 2);
    ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
    ctx.fill();
}
}
}
}
}

    // Hintergrundsterne
    const stars = [];

    // Sterne generieren
    for (let i = 0; i < 50; i++) {
    stars.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        size: Math.random() * 1 + 0.5,
        blinkSpeed: Math.random() * 0.05 + 0.01
    });
}

    // Sterne zeichnen
    function drawStars() {
    for (const star of stars) {
    const blinkFactor = Math.sin(Date.now() * star.blinkSpeed) * 0.5 + 0.5;
    ctx.beginPath();
    ctx.arc(star.x, star.y, star.size, 0, Math.PI * 2);
    ctx.fillStyle = `rgba(255, 255, 255, ${blinkFactor * 0.7})`;
    ctx.fill();
}
}

    // Animation
    function animate() {
    // Canvas teilweise löschen für Trail-Effekt
    ctx.fillStyle = 'rgba(10, 14, 32, 0.15)';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Hintergrundsterne zeichnen
    drawStars();

    // Globale Effekte aktualisieren
    pulsePhase += 0.02;
    hue = (hue + 0.1) % 360;
    flowFieldTime += 0.003;

    // Periodische Änderung der max. Verbindungsdistanz
    maxDistance = baseMaxDistance * (Math.sin(pulsePhase * 0.2) * 0.2 + 1);
    globalAlpha = Math.sin(pulsePhase * 0.3) * 0.2 + 0.5;

    // Verbindungen zeichnen
    drawConnections();

    // Partikel aktualisieren und zeichnen
    for (const particle of particles) {
    particle.update();
    particle.draw();
}

    requestAnimationFrame(animate);
}

    // Mausinteraktion
    canvas.addEventListener('mousemove', (event) => {
    mouseX = event.clientX;
    mouseY = event.clientY;
});

    canvas.addEventListener('mouseout', () => {
    mouseX = null;
    mouseY = null;
});

    // Touch-Interaktion für mobile Geräte
    canvas.addEventListener('touchstart', (event) => {
    event.preventDefault();
    mouseX = event.touches[0].clientX;
    mouseY = event.touches[0].clientY;
});

    canvas.addEventListener('touchmove', (event) => {
    event.preventDefault();
    mouseX = event.touches[0].clientX;
    mouseY = event.touches[0].clientY;
});

    canvas.addEventListener('touchend', () => {
    mouseX = null;
    mouseY = null;
});

    // Bei Größenänderung des Fensters Canvas anpassen
    window.addEventListener('resize', () => {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    // Sterne neu positionieren
    for (const star of stars) {
    star.x = Math.random() * canvas.width;
    star.y = Math.random() * canvas.height;
}
});
    
    document.addEventListener('DOMContentLoaded', function() {
        const element = document.getElementById('demo-element');
        const speedControl = document.getElementById('speed');
        const glowControl = document.getElementById('glow');
        const widthControl = document.getElementById('width');
        const presets = document.querySelectorAll('.preset');

        // Animation starten
        animate();
        
        // Funktion zum Aktualisieren der Animation
        function updateAnimation() {
            const speed = 21 - speedControl.value; // Umkehrung damit höhere Werte schneller sind
            const glow = glowControl.value;
            const width = widthControl.value;

            element.style.setProperty('--animation-speed', `${speed}s`);
            document.documentElement.style.setProperty('--animation-speed', `${speed}s`);

            // CSS wird direkt angewendet
            element.style.cssText = `
                    --glow-strength: ${glow}px;
                    --border-width: ${width}px;
                `;

            // Dynamisch erstellte Styles für komplexere Änderungen
            const styleElement = document.getElementById('dynamic-styles') || document.createElement('style');
            styleElement.id = 'dynamic-styles';

            styleElement.textContent = `
                    .rainbow-border::before {
                        filter: blur(${glow}px);
                        animation: rainbowGlow ${speed}s linear infinite;
                    }
                    
                    .rainbow-border::after {
                        top: ${width}px;
                        left: ${width}px;
                        right: ${width}px;
                        bottom: ${width}px;
                    }
                `;

            if (!document.getElementById('dynamic-styles')) {
                document.head.appendChild(styleElement);
            }
        }

        // Farbschemata
        const colorSchemes = {
            rainbow: `linear-gradient(
                    45deg, 
                    red, orange, yellow, green, blue, indigo, violet, 
                    red, orange, yellow, green, blue, indigo, violet
                )`,
            fire: `linear-gradient(
                    45deg, 
                    #ff0000, #ff8000, #ffff00, 
                    #ff0000, #ff8000, #ffff00
                )`,
            ocean: `linear-gradient(
                    45deg, 
                    #0077be, #00ccff, #98f5ff, 
                    #0077be, #00ccff, #98f5ff
                )`,
            neon: `linear-gradient(
                    45deg, 
                    #ff00ff, #00ffff, #ff00ff, #00ffff, 
                    #ff00ff, #00ffff, #ff00ff, #00ffff
                )`
        };

        // Event-Listeners für Steuerelemente
        speedControl.addEventListener('input', updateAnimation);
        glowControl.addEventListener('input', updateAnimation);
        widthControl.addEventListener('input', updateAnimation);

        // Event-Listeners für Farbschema-Presets
        presets.forEach(preset => {
            preset.addEventListener('click', function() {
                const scheme = this.classList[1]; // Klassennamen als Identifier nutzen

                const styleElement = document.getElementById('color-styles') || document.createElement('style');
                styleElement.id = 'color-styles';

                styleElement.textContent = `
                        .rainbow-border::before {
                            background: ${colorSchemes[scheme]};
                            background-size: 400% 400%;
                        }
                    `;

                if (!document.getElementById('color-styles')) {
                    document.head.appendChild(styleElement);
                }
            });
        });

        // Anfängliche Anwendung der Einstellungen
        updateAnimation();
    });