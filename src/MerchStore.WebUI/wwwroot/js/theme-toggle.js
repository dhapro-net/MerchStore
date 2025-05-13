document.addEventListener('DOMContentLoaded', function () {
    // Always start in light mode
    document.body.classList.remove('dark-mode');
    document.body.classList.add('light-mode');
    localStorage.removeItem('theme');

    const themeToggle = document.getElementById('theme-toggle-btn');
    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const isLight = document.body.classList.contains('light-mode');
            document.body.classList.toggle('light-mode', !isLight);
            document.body.classList.toggle('dark-mode', isLight);
            localStorage.setItem('theme', isLight ? 'dark' : 'light');
        });
    }
});
