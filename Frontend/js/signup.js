document.addEventListener("DOMContentLoaded", () => {
    const signupBtn = document.getElementById("signupBtn");

    signupBtn.addEventListener("click", signup);
});

function signup() {
    window.location.href = "register.html";
}