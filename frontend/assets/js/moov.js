var menu = document.getElementById('menu');
var menu_slide = document.getElementById('menu_slide');
menu.addEventListener('click', function () {
    permute(menu_slide);
})

function permute(menu_slide){
    menu_slide.classList.toggle('menu2');
    menu_slide.classList.toggle('menu');
}