document.addEventListener("DOMContentLoaded", () => {
    viewUserProfile();
});

function viewUserProfile() {
    apiRequest("/profile", {
        method: "GET"
    })
    .then(result => {
        console.log(result);
        document.getElementById("profileView").innerHTML = `
            <h2>Hi! ${result.firstName} ${result.lastName}</h2>
            <p>Welcome to your profile page.</p>
            <h4>You live in - ${result.address}</h4>
            <h4>Your Phone Number is - ${result.phoneNumber}</h4>
        `;
        var btn = document.createElement("button");
        btn.addEventListener("click", () => {
            window.location.href = "edit-profile.html";
        });
        btn.textContent = "Update Profile";
        document.body.appendChild(btn);
    })
    .catch(error => {
        alert("Failed to determine profile status");
    });
}
