/*
Tabla de registro de peustos
*/

class Puesto {
    constructor() {
        this.id = -1;
        this.nombre = '';
        this.dependenciaId = -1;
        this.activo = true;

        this.dependenciaNombre = '';
    }

    copyValues(item) {
        this.id = item.id;
        this.nombre = item.nombre;
        this.dependenciaId = item.dependenciaId;
        this.activo = item.activo;

        this.dependenciaNombre = item.dependenciaNombre;

        return this;
    }

    reset() {
        this.id = -1;
        this.nombre = '';
        this.dependenciaId = -1;
        this.activo = true;

        this.dependenciaNombre = '';
        $('#dependenciaId').val('');
    }

    updateValues(original, item) {
        original.id = item.id;
        original.nombre = item.nombre;
        original.dependenciaId = item.dependenciaId;
        original.activo = item.activo;

        original.dependenciaNombre = item.dependenciaNombre;

        return original;
    }
}

var model;
var grid;

$(document).ready(function () {
    model = new Vue({
        el: '#appVue',
        data: {
            puestos: [],
            current: new Puesto(),
        },
        created() {
            var me = this;
            showSpinner();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: '/Municipio/PuestosGet',
                success: function (result, status, xhr) {

                    hideSpinner();
                    if (result.success == false) {
                        alertify.error(result.error);
                        return;
                    }

                    $.each(result.model, function () {
                        me.puestos.push(new Puesto().copyValues(this));
                    });

                    //var selectChoices = new Choices('#dependenciaId', {
                    //    shouldSort: false,
                    //    placeholder: true,
                    //    searchPlaceholderValue: false,
                    //    itemSelectText: '',
                    //});


                    //selectChoices.setValue(result.dependencias);

                    var s = '<option placeholder="" value="">Seleccione un registro</option>';
                    $.each(result.dependencias, function () {
                        s += '<option value="' + this.value + '">' + this.label + '</option>';
                    });
                    $("#dependenciaId").html(s);

                    setTimeout(function () {
                        const selectChoices = new CWhoices('#dependenciaId',
                            {
                                shouldSort: false,
                                placeholder: true,
                                searchPlaceholderValue: false,
                                itemSelectText: ''
                            });
                    }, 500);

                    $("#dependenciaId").change(function () {
                        me.current.dependenciaId = $(this).find(":selected").val();
                        me.current.dependenciaNombre = $(this).find(":selected").text();
                    });

                    grid = new gridjs.Grid({
                        columns:
                            [
                                {
                                    id: 'id',
                                    hidden: true
                                },
                                {
                                    id: 'nombre',
                                    name: 'Nombre'
                                },
                                {
                                    id: 'dependenciaId',
                                    hidden: true
                                },
                                {
                                    id: 'activo',
                                    name: 'Activo',
                                    formatter: (_, row) => row.cells[3].data == true ? 'Activo' : 'Inactivo'
                                },
                                {
                                    id: 'dependenciaNombre',
                                    name: 'Dependencia'
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
                        data: me.puestos,
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
                    url: '/Municipio/PuestoSave',
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
                            data: me.puestos
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
                var puesto = this.puestos.find(obj => obj.id == result.model.id);
                puesto.updateValues(puesto, result.model);
            },
            agregaRegistro: function (result) {
                this.puestos.push(new Puesto().copyValues(result.model));
            },
            eliminaRegistro: function (id) {
                var me = this;
                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: { id: id },
                    url: '/Municipio/PuestoDelete',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        let index = me.puestos.map((item) => item.id).indexOf(id);
                        if (index >= 0) {
                            me.puestos.splice(index, 1);

                            grid.updateConfig({
                                data: me.puestos
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
    if (model.current.nombre && model.current.nombre.length > 0 &&
        model.current.dependenciaId && parseInt(model.current.dependenciaId.toString()) > 0)
        return true;

    alertify.warning('Debe indicar todos los campos del registro');
    return false;
}

function editaRegistro(id) {

    if (id < 0) {
        return;
    }

    var puesto = model.puestos.find(obj => obj.id == id);
    if (puesto) {
        model.current = new Puesto().updateValues(model.current, puesto);
        $('#dependenciaId').val(model.current.dependenciaId);
    }
}

function eliminaRegistro(id) {
    if (id <= 0) {
        return;
    }

    Swal.fire({
        title: "¿Confirma que desea eliminar el puesto seleccionado?",
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