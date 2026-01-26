document.addEventListener("DOMContentLoaded", () => {
    viewUserProfile();
});

function viewUserProfile() {
    apiRequest("/profile", {
        method: "GET"
    })
    .then(res => {

        if (!res.success) {
            alert(res.message || "View failed");
            return;
        }
        renderProfile(res.data);
        //console.log(res);

    })
    .catch(error => {
        alert(error.message || "Failed to determine profile status");
    });
}

function renderProfile(data) {
    document.getElementById("profileView").innerHTML = `
            <h2>Hi! ${data.firstName} ${data.lastName}</h2>
            <p>Welcome to your profile page.</p>
            <h4>You live in - ${data.address}</h4>
            <h4>Your Phone Number is - ${data.phoneNumber}</h4>
        `;
        var btn = document.createElement("button");
        btn.addEventListener("click", () => {
            window.location.href = "edit-profile.html";
        });
        btn.textContent = "Update Profile";
        document.body.appendChild(btn);
}