/*
Tabla de registro de servicios
*/

class Servicio {
    constructor() {
        this.id = -1;
        this.clave = '';
        this.nombre = '';
        this.descripcion = '';
        this.partidaDetalleId = -1;
        this.partidaDetalleNombre = '';
    }

    copyValues(item) {
        this.id = item.id;
        this.clave = item.clave;
        this.nombre = item.nombre;
        this.descripcion = item.descripcion;
        this.partidaDetalleId = item.partidaDetalleId;
        this.partidaDetalleNombre = item.partidaDetalleNombre;

        return this;
    }

    reset() {
        this.id = -1;
        this.clave = '';
        this.nombre = '';
        this.descripcion = '';
        this.partidaDetalleId = -1;
        this.partidaDetalleNombre = '';
    }

    updateValues(original, item) {
        original.id = item.id;
        original.clave = item.clave;
        original.nombre = item.nombre;
        original.descripcion = item.descripcion;
        original.partidaDetalleId = item.partidaDetalleId;
        original.partidaDetalleNombre = item.partidaDetalleNombre;

        return original;
    }
}

var model;
var grid;

$(document).ready(function () {
    model = new Vue({
        el: '#appVue',
        data: {
            servicios: [],
            current: new Servicio()
        },
        created() {
            var me = this;
            showSpinner();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: '/Compras/ServiciosGet',
                success: function (result, status, xhr) {

                    hideSpinner();

                    if (result.success == false) {
                        alertify.error(result.error);
                        return;
                    }

                    $.each(result.servicios, function () {
                        me.servicios.push(new Servicio().copyValues(this));
                    })

                    grid = new gridjs.Grid({
                        columns:
                            [
                                {
                                    id: 'id',
                                    hidden: true
                                },
                                {
                                    id: 'clave',
                                    name: 'Clave'
                                },
                                {
                                    id: 'nombre',
                                    name: 'Nombre'
                                },
                                {
                                    id: 'descripcion',
                                    name: 'Descripcion'
                                },
                                {
                                    id: 'partidaDetalleNombre',
                                    name: 'Partida'
                                },
                                {
                                    name: "Acciones",
                                    sort: {
                                        enabled: false
                                    },
                                    formatter: (cell, row) => {
                                        return gridjs.html('<div class="d-flex gap-3"><a href="javascript:void(0);" onclick="editaRegistro(' + row.cells[0].data + ');" data-bs-toggle="modal" data-bs-target=".modalEdit" data-bs-toggle="tooltip" data-bs-placement="top" title="Editar" class="text-success"><i class="mdi mdi-pencil font-size-18"></i></a><a href="javascript:void(0);" onclick="eliminaRegistro(' + row.cells[0].data + ');" data-bs-toggle="tooltip" data-bs-placement="top" title="Delete" class="text-danger"><i class="mdi mdi-delete font-size-18"></i></a></div>');
                                    }
                                }
                            ],
                        pagination: {
                            limit: 8
                        },
                        sort: true,
                        search: true,
                        data: me.servicios,
                        language: {
                            'search': {
                                'placeholder': '🔍 Buscar...'
                            },
                            'pagination': {
                                'previous': 'Anterior',
                                'next': 'Siguiente',
                                'showing': 'Mostrando',
                                'results': () => 'Registros',
                                'to': 'a',
                                'of': 'de'
                            }
                        }
                    }).render(document.getElementById("table-catalog"));

                    $('#btnAdd').show();
                },
                error: function (error) {
                    hideSpinner();
                }
            });
        },
        methods: {
            guardaRegistro: function () {
                var me = this;

                if (!validaRegistro()) {
                    return;
                }

                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: model.current,
                    url: '/Compras/ServicioSave',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        if (me.current.id > 0) {
                            me.actualizaRegistro(result);
                        } else {
                            me.agregaRegistro(result);
                        }

                        grid.updateConfig({
                            data: me.servicios
                        }).forceRender();

                        $('#modalEdit').modal('hide');

                        alertify.success('Guardado exitoso');
                    },
                    error: function (error) {
                        hideSpinner();
                    }
                });
            },
            actualizaRegistro: function (result) {
                var servicio = this.servicios.find(obj => obj.id == result.model.id);
                servicio.updateValues(servicio, result.model);
            },
            agregaRegistro: function (result) {
                this.servicios.push(new Servicio().copyValues(result.model));
            },
            eliminaRegistro: function (id) {
                var me = this;
                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: { id: id },
                    url: '/Compras/ServicioDelete',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        let index = me.servicios.map((item) => item.id).indexOf(id);
                        if (index >= 0) {
                            me.servicios.splice(index, 1);

                            grid.updateConfig({
                                data: me.servicios
                            }).forceRender();
                        }
                    },
                    error: function (error) {
                        hideSpinner();
                    }
                });
            }
        }
    });
})

function validaRegistro() {
    if (model.current.clave && model.current.clave.length > 0 &&
        model.current.nombre && model.current.nombre.length > 0 &&
        model.current.descripcion && model.current.descripcion.length > 0)
        return true;

    alertify.warning('Debe indicar todos los campos del registro');
    return false;
}

function editaRegistro(id) {

    if (id < 0) {
        return;
    }

    var servicio = model.servicios.find(obj => obj.id == id);
    if (servicio) {
        model.current = new Servicio().updateValues(model.current, servicio);
    }
}

function eliminaRegistro(id) {
    if (id <= 0) {
        return;
    }

    Swal.fire({
        title: "¿Confirma que desea eliminar el servicio seleccionado?",
        text: "El proceso no es reversible!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#51d28c",
        cancelButtonColor: "#f34e4e",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar"
    }).then(function (result) {
        if (result.value) {
            model.eliminaRegistro(id);
        }
    });
}