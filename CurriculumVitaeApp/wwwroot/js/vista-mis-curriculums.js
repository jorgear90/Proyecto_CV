//Este js es de la vista MisCurriculums

// Alerta que solicita que el usuario confirme la eliminación de un cv y su respectivo registro. 
function confirmarEliminacion(event) {
    event.preventDefault();

    Swal.fire({
        title: '¿Está seguro que desea eliminar este cv?',
        text: 'Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                icon: 'success',
                title: 'Cv eliminado',
                text: 'El cv se ha eliminado con éxito.',
                confirmButtonColor: '#005D8F',
                allowOutsideClick: false,
                allowEscapeKey: false
            }).then(() => {
                document.getElementById('submitButtonHidden').value = 'eliminar';
                event.target.closest('form').submit();
            });
        }

    });
}