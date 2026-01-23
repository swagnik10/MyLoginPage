(function () {
    const userId = localStorage.getItem("userId");

    if (!userId) {
        window.location.href = "index.html";
    }
})();
