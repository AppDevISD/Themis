function pageInit()
{
    var storageField = document.getElementById("colorModeToggle");
    var theme = localStorage["theme"];
    storageField.classList.add(theme);
    return theme
}

function supports_html5_storage()
{
    try {
        return 'localStorage' in window && window['localStorage'] !== null;
    }
    catch (e)
    {
        return false;
    }
}

function saveTheme(theme)
{
    localStorage["theme"] = theme;
}

function toggleTheme(theme)
{
    var colorTheme = document.getElementById("colorTheme");
    var buttonToggle = document.getElementById("colorModeToggle")

    if (theme == "darkmode") {
        colorTheme.innerHTML = ":root {--gradient-base: #474747; --gradient-dark: #2d2d2d; --dark-gray: #1b1b1b; --text-color: #ffffff; --hover-bg: white; --list-border: #2d2d2d; --header-bg: #111111; --active-bg: #558395; --active-txt: #ffffff;}";
        buttonToggle.setAttribute("class", "fas fa-sun");
        buttonToggle.setAttribute("onclick", "toggleTheme('lightmode')");
        localStorage["theme"] = "darkmode";
    }
    else
    {
        colorTheme.innerHTML = ":root {--gradient-base: #ffffff; --gradient-dark: #d2d2d2; --dark-gray: #c1c1c1; --text-color: #000000; --hover-bg: #2d2d2d; --list-border: #e9e9e9; --header-bg: #9b9b9b; --active-bg: #558395; --active-txt: #ffffff;}";
        buttonToggle.setAttribute("class", "fas fa-moon");
        buttonToggle.setAttribute("onclick", "toggleTheme('darkmode')");
        localStorage["theme"] = "lightmode";
    }
}

function ActivatePage(page)
{
    previousPages = document.getElementsByClassName("active");
    for (let i = 0; i < previousPages.length; i++)
    {
        previousPages[i].classList.remove("active");
    }
    activePage = document.getElementById(page);
    activePage.classList.add("active");

}