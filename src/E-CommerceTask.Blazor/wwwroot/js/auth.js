// Set cookie
window.setAuthCookie = function(token, days, cookieName) {
    try {
        const expires = new Date();
        expires.setTime(expires.getTime() + (days * 24 * 60 * 60 * 1000));
        document.cookie = `${cookieName}=${token};expires=${expires.toUTCString()};path=/;SameSite=Strict;Secure`;
        return true;
    } catch (e) {
        console.error('Failed to set auth cookie:', e);
        return false;
    }
}

// Remove cookie
window.removeAuthCookie = function(cookieName) {
    try {
        document.cookie = `${cookieName}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;SameSite=Strict;Secure`;
        return true;
    } catch (e) {
        console.error('Failed to remove auth cookie:', e);
        return false;
    }
}