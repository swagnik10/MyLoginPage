(function () {
    const userId = localStorage.getItem("accessToken");

    if (!userId) {
        handleUnauthorized();
    }
})();

function handleUnauthorized() {
    localStorage.removeItem("accessToken");
    window.location.href = "index.html";
}
