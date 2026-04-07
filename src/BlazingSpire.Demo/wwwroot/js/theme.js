window.blazingSpire = {
    getTheme: function () {
        return document.documentElement.classList.contains('dark');
    },
    setTheme: function (isDark) {
        if (isDark) {
            document.documentElement.classList.add('dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('theme', 'light');
        }
    }
};
