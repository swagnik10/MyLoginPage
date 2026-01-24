function logout() {
    // Clear the authentication token from local storage
    localStorage.removeItem('userId');
    // Redirect the user to the login page
    window.location.href = 'index.html';
}