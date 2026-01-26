function logout() {
    // Clear the authentication token from local storage
    localStorage.removeItem('accessToken');
    // Redirect the user to the login page
    window.location.href = 'index.html';
}