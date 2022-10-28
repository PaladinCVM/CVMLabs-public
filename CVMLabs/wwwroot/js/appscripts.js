const links = document.querySelectorAll('.nav-link');

links.forEach(link => {
    if (link.getAttribute('href') === window.location.pathname) link.classList.toggle('active');
});