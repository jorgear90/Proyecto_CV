//Este js es para las vistas Crear de:
    //Conocimientos
    //DatosBasicos
    //ExperienciaLaboral
    //FormacionAcademica
    //Habilidades
    //Links

// Método que confirma la correcta creación de un registro
function confirmarCreacion(event) {
    event.preventDefault();

    var form = $("#formCrear");

    // fuerza validación MVC
    if (!form.valid()) {

        // buscamos error específico de Enlace
        var errorEnlace = $('span[data-valmsg-for="Enlace"]').text();

        if (errorEnlace && errorEnlace.toLowerCase().includes("url")) {
            Swal.fire({
                icon: 'error',
                title: 'Enlace inválido',
                text: 'El campo Enlace debe ser una URL válida.',
                confirmButtonColor: '#d33'
            });
        }

        return;
    }

    Swal.fire({
        icon: 'success',
        title: 'Registro creado',
        text: 'El registro se ha creado con éxito.',
        confirmButtonColor: '#005D8F',
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then(() => {
        document.getElementById('submitButtonHidden').value = 'crear';
        document.getElementById('formCrear').submit();
    });
}
