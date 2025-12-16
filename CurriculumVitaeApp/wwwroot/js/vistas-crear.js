//Este js es para las vistas Crear de:
    //Conocimientos
    //DatosBasicos
    //ExperienciaLaboral
    //FormacionAcademica
    //Habilidades

function confirmarCreacion(event) {
    event.preventDefault();

    var form = $("#formCrear");

    if (!form.valid()) {
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