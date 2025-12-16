//Este js es para la vista crear de Usuarios

$(function () {
    if (window.usuarioCreado === true) {
        Swal.fire({
            icon: 'success',
            title: 'Usuario creado',
            text: 'El usuario se ha creado con éxito.',
            confirmButtonColor: '#005D8F',
            allowOutsideClick: false,
            allowEscapeKey: false
        }).then(() => {
            window.location.href = window.loginUrl;
        });
    }
});