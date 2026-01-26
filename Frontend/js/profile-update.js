document.addEventListener("DOMContentLoaded", async () => {
    const profileData = await getUserProfile();

    document.getElementById("submitBtn")
        .addEventListener("click", () => updateProfile(profileData));

    document.getElementById("credentialBtn")
        .addEventListener("click", () => window.location.href = "edit-credential.html");

    document.getElementById("cancelBtn")
        .addEventListener("click", () => window.location.href = "profile-view.html");
});

// we are not able to return like this beacuse it is inside a promise. To get the value outside, we have to use then 
// const profileData = getUserProfile(); --runs immediately, before the API call completes, and receives: undefined
// way to do :
/*      getUserProfile().then(profileData => {
            console.log(profileData);
        });
*/

/*function getUserProfile() {
    apiRequest("/profile", {
        method: "GET"
    })
    .then(result => {
        if (!result.success) {
            alert(result.message || "Update failed");
            return;
        }

        const existingData = existingProfileData(result.data);

        document.getElementById("firstName").value = existingData.firstName;
        document.getElementById("lastName").value = existingData.lastName;
        document.getElementById("address").value = existingData.address;
        document.getElementById("phoneNumber").value = existingData.phoneNumber;

        console.log(existingData);

        return existingData;
    })
    .catch(error => {
        alert(error.message || "Failed to determine profile data");
    });
}*/

async function getUserProfile() {
    const result = await apiRequest("/profile", {
        method: "GET"
    });

    if (!result.success) {
        alert(result.message || "Get profile data failed");
        return;
    }

    const existingData = existingProfileData(result.data);

    document.getElementById("firstName").value = existingData.firstName;
    document.getElementById("lastName").value = existingData.lastName;
    document.getElementById("address").value = existingData.address;
    document.getElementById("phoneNumber").value = existingData.phoneNumber;

    return existingData;
}


function updateProfile(profileData) {

    const newProfileData = {
        firstName: document.getElementById("firstName").value.trim(),
        lastName: document.getElementById("lastName").value.trim(),
        address: document.getElementById("address").value.trim(),
        phoneNumber: document.getElementById("phoneNumber").value.trim()
    };

    if (!newProfileData.firstName || !newProfileData.lastName || !newProfileData.address || !newProfileData.phoneNumber) {
        alert("All fields are required");
        return;
    }
    //checking if any data has changed
    if (!hasProfileDataChanged(profileData, newProfileData)) {
        alert("No changes detected in profile data");
        return;
    }

    apiRequest("/profile", {
        method: "PUT",
        body: JSON.stringify({
            firstName: newProfileData.firstName,
            lastName: newProfileData.lastName,
            address: newProfileData.address,
            phoneNumber: newProfileData.phoneNumber
        })
    })
    .then((res) => {
        if (!res.success) {
            alert(res.message || "Update failed");
            return;
        }
        window.location.href = "profile-view.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}

function existingProfileData(data) {
    const existingData = {
        firstName: data.firstName,
        lastName: data.lastName,
        address: data.address,
        phoneNumber: data.phoneNumber
    }
    return existingData;
}

function hasProfileDataChanged(originalData, newData) {
    return originalData.firstName !== newData.firstName ||
           originalData.lastName !== newData.lastName ||
           originalData.address !== newData.address ||
           originalData.phoneNumber !== newData.phoneNumber;
}
