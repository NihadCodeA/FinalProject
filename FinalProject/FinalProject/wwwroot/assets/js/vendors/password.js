var password = document.getElementById("password"),
    passwordtoggler = document.getElementById("passwordToggler");
showHidePassword = () => {
    "password" == password.type ? (password.setAttribute("type", "text"),
        passwordtoggler.classList.add("bi-eye"), passwordtoggler.classList.remove("bi-eye-slash")) :
        (passwordtoggler.classList.remove("bi-eye"), passwordtoggler.classList.add("bi-eye-slash"),
            password.setAttribute("type", "password"))
}, passwordtoggler.addEventListener("click", showHidePassword);

var confirmPassword = document.getElementById("confirmPassword"),
    confirmPasswordtoggler = document.getElementById("confirmPasswordToggler");
showHideConfirmPassword = () => {
    "password" == confirmPassword.type ? (confirmPassword.setAttribute("type", "text"),
        confirmPasswordtoggler.classList.add("bi-eye"), confirmPasswordtoggler.classList.remove("bi-eye-slash")) :
        (confirmPasswordtoggler.classList.remove("bi-eye"), confirmPasswordtoggler.classList.add("bi-eye-slash"),
            confirmPassword.setAttribute("type", "password"))
}, confirmPasswordtoggler.addEventListener("click", showHideConfirmPassword);