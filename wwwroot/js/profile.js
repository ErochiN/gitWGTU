// Загрузка данных профиля с сервера и вывод их на страницу.

async function loadProfile() {
    const response = await fetch("/api/profile");

    // Не авторизован: отправляем на страницу входа
    if (response.status === 401) {
        window.location.href = "/auth/login";
        return;
    }

    if (!response.ok) {
        document.getElementById("user-name").textContent =
            "Не удалось загрузить профиль";
        return;
    }

    const user = await response.json();

    document.getElementById("user-name").textContent =
        `${user.firstName} ${user.lastName}`;

    document.getElementById("user-email").textContent =
        user.email;

    document.getElementById("user-group").textContent =
        user.group;

}

loadProfile();