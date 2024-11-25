//js activa elemento escucha cuando el DOM carga
document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.getElementById("mainSidebar"); //referencia a la barra lateral

    const handleSidebarVisibility = () => {
        if (window.innerWidth >= 992) {
            //ocultar sidebar en pantallas grandes
            sidebar.style.display = "none";
        } else {
            //mostrar sidebar en pantallas pequeñas
            sidebar.style.display = "block";
        }
    };

    //ejecutar al cargar
    handleSidebarVisibility();

    //escuchar cambios en el tamaño de la ventana
    window.addEventListener("resize", handleSidebarVisibility);
});