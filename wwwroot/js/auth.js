// Общий скрипт для страниц входа и регистрации.
(function () {
    // 1. Показываем ошибку, если сервер вернул редирект вида ?error=код
    const messages = {
        exists:      'Эта почта уже зарегистрирована. Войдите или используйте другую.',
        invalid:     'Проверьте введённые данные и попробуйте снова.',
        credentials: 'Неверная почта или пароль.'
    };

    const errorBox = document.getElementById('form-error');
    const code = new URLSearchParams(window.location.search).get('error');

    if (errorBox && code) {
        errorBox.textContent = messages[code] || 'Что-то пошло не так. Попробуйте ещё раз.';
        errorBox.hidden = false;
    }

    // 2. На странице регистрации проверяем, что пароли совпадают
    const password = document.getElementById('password');
    const confirm  = document.getElementById('confirmPassword');

    if (password && confirm) {
        const check = () => {
            const mismatch = confirm.value !== '' && confirm.value !== password.value;
            confirm.setCustomValidity(mismatch ? 'Пароли не совпадают' : '');
        };
        password.addEventListener('input', check);
        confirm.addEventListener('input', check);
    }
})();


const loginForm = document.getElementById('login-form');

