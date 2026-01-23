document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("submitBtn")
        .addEventListener("click", submitProfile);
});

function submitProfile() {
    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const address = document.getElementById("address").value.trim();
    const phoneNumber = document.getElementById("phoneNumber").value.trim();

    if (!firstName || !lastName || !address || !phoneNumber) {
        alert("All fields are required");
        return;
    }

    apiRequest("/profile", {
        method: "POST",
        body: JSON.stringify({
            firstName,
            lastName,
            address,
            phoneNumber
        })
    })
    .then(() => {
        window.location.href = "profile-view.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}
