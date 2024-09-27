<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="View.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.View" %>
<%@ Register Src="~/desktopmodules/ReservasExportaciones/Components/Controls/ControlObservations.ascx" TagPrefix="uc1" TagName="ControlObservations" %>

<link rel="stylesheet" type="text/css" href="/DesktopModules/ReservasExportaciones/vendor/DataTables/datatables.min.css" />
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
<script type="text/javascript" src="/DesktopModules/ReservasExportaciones/vendor/DataTables/datatables.min.js"></script>
<script>
    const DEBUG = true;
    var moduleScope = $('#<%=phMain.ClientID %>');
    var self = moduleScope;
    const moduleId = <%=ModuleId%>;
    const userID = <%=UserId%>;
    const portalID =<%=PortalId%>;
    const cn_hash = "<%=ConnectionString%>";
    const pkey = "<%=PublicKey%>";
    const isAgent = "<%=IsAgente%>";
    var nitUser = "<%=NitUser%>";
    const sf = $.ServicesFramework(moduleId);
    const urlApi = sf.getServiceRoot('ReservasExportaciones');
    const i18 = {
        create: {
            button: "Agregar",
            title: "Crear Registro",
            submit: "Agregar registro"
        },
        edit: {
            button: "Editar",
            title: "Editar Registro",
            submit: "Actualizar"
        },
        remove: {
            button: "Eliminar",
            title: "Eliminar",
            submit: "Eliminar",
            confirm: {
                _: "Esta seguro de eliminar %d registro?",
                1: "Esta seguro de eliminar el registro?"
            }
        },
        error: {
            system: "Ups!!! Ocurrio un error!!!"
        },
        datetime: {
            previous: 'Anterior',
            next: 'Siguiente',
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            weekdays: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab']
        }
    };
    const lang = {
        "sProcessing": "Procesando...",
        "sLengthMenu": "Mostrar _MENU_ registros",
        "sZeroRecords": "No se encontraron resultados",
        "sEmptyTable": "Ning&uacute;n dato disponible en esta tabla",
        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
        "sInfoPostFix": "",
        "sSearch": "Filtrar:",
        "sUrl": "",
        "sInfoThousands": ",",
        "sLoadingRecords": "Cargando...",
        "oPaginate": {
            "sFirst": "Primero",
            "sLast": "Último",
            "sNext": "Siguiente",
            "sPrevious": "Anterior"
        },
        "oAria": {
            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
        },
        "buttons": {
            "copy": "Copiar",
            "colvis": "Visibilidad"
        }
    };
    var editor = {};
    var editorCntr = {};
    var table = {};
    var tableChild = {};
    var tableFiles = {};
    var tableCntr = {};
    var booking = '';
    var btn_child = {};
    var tableComments = {};
    var row2 = null;
    var bl_prueba = '';
    
    function CambiarEstado(bl, control) {
        var selectedOptionId = control.options[control.selectedIndex].id;
        $.ajax({
            url: urlApi + 'Bookings/UpdateState',
            beforeSend: sf.setModuleHeaders,
            data: {
                cn: cn_hash,
                pkey: pkey,
                nituser: nitUser,
                state_id: selectedOptionId,
                isagent: isAgent,
                Booking: bl
            },
            type: 'POST',
            success: function (response) {
                console.log(response);
                alert("estado actualizado");
            },
            error: function (error) {
                console.error(error);
            }
        });
    }

    function leerDocumentos(bl) {
        tableFiles = $('#tbl_files').DataTable({
            dom: 'Bfrtip',
            serverSide: true,
            processing: true,
            destroy: true,
            order: [1, 'asc'],
            ajax: {
                url: urlApi + 'FilesExpo/Get',
                beforeSend: sf.setModuleHeaders,
                data: function (data) {
                    data.cn = cn_hash;
                    data.pkey = pkey;
                    data.Booking = bl;
                    data.nituser = nitUser;
                    data.isagent = isAgent;
                    return data;
                },
                type: 'POST'
            },
            columns: [
                {
                    className: 'dt-control',
                    orderable: false,
                    searchable: false,
                    data: null,
                    defaultContent: '',
                    width: '10%'
                },
                {
                    title: "RESERVA",
                    data: "BOOKINGS_EXPO_DOCS.BOOKING",
                    key: true
                },
                {
                    title: "CODIGO",
                    data: "BOOKINGS_EXPO_DOCS.CODIGO"
                },
                {
                    title: "CATEGORIA",
                    data: "BOOKINGS_EXPO_DOCS.CATEGORIA"
                },
                {
                    title: "ADJUNTO",
                    data: "BOOKINGS_EXPO_DOCS.ADJUNTO",
                    render: function (file_id) {
                        return file_id ?
                            `<a href="${editor.file('ARCHIVOS_BOOKINGS', file_id).web_path}" target="_blank">Ver</a>` :
                            null;
                    },
                    defaultContent: "",
                }

            ],
            select: {
                style: 'os',
                selector: 'td:not(:first-child)'
            },

            lengthChange: false,
            language: lang,
            buttons: [
                {
                    extend: 'create', editor: editor,
                    className: 'btn btn-primary',
                    formButtons: [
                        {
                            label: 'Crear registro',
                            className: 'btn btn-success btn-crear',
                            fn: function () {
                                this.submit();
                            }
                        },
                        {
                            label: 'Cerrar ventana',
                            className: 'btn btn-danger',
                            fn: function () {
                                this.close();
                            }
                        }
                    ]

                },
                { extend: 'edit', className: 'btn btn-primary', editor: editor },
                { extend: 'remove', className: 'btn btn-primary', editor: editor }

            ]
        });




        tableFiles.off('click', 'td.dt-control').on('click', 'td.dt-control', function () {

            let tr = $(this).closest('tr');

            btn_child = $(this);

            let row = tableFiles.row(tr);

            if (row.child.isShown()) {

                destroyChild(row);
                tr.removeClass('shown');

            }
            else {
                // Open this row
                createChild(row);
                tr.addClass('shown');
            }
        });


        function createChild(row) {
            tableChild = $('<table cellpadding="0" cellspacing="0" border="0" class="display" width="100%"/>');


            editorCntr = new DataTable.Editor({
                ajax: {
                    create: {
                        type: 'POST',
                        url: urlApi + 'Cntr/Create',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                            data.ID = row.data().ID;
                            return data;
                        }
                    },
                    edit: {
                        type: 'PUT',
                        url: urlApi + 'Cntr/Edit',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                            data.ID = row.data().ID;
                            return data;
                        }
                    },
                    remove: {
                        type: 'POST',
                        url: urlApi + 'Cntr/Remove',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            /*  data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';*/
                            data.ID = row.data().ID;
                            return data;
                        }
                    }
                },
                table: tableChild,
                fields: [
                    {
                        "label": "CONTENEDOR:",
                        "name": "DETCARGAB.IDEN",

                    },
                    {
                        "label": "BULTOS:",
                        "name": "DETCARGAB.BULTOS",
                        "attr": {
                            type: 'number'
                        }
                    },
                    {
                        "label": "PESO:",
                        "name": "DETCARGAB.PESO_ORIG",
                        "attr": {
                            type: 'number'
                        }
                    },
                    {
                        "label": "ID:",
                        "name": "DETCARGAB.ID_BOOKINGS_EXPO",
                        "type": "hidden"

                    }
                ],
                i18n: i18
            });
            btn_child.prop('disabled', true);
            row.child(tableChild).show();

            fetch(urlApi + 'Cntr/Get2', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json', // Establece el tipo de contenido a JSON
                    // Otros encabezados si son necesarios
                },
                body: JSON.stringify({
                    cn: cn_hash,
                    pkey: pkey,
                    nituser: nitUser,
                    isagent: isAgent,
                    Booking: booking,
                    ID: row.data().ID
                })
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }

                    return response.json();
                })
                .then(function (data) {
                    btn_child.prop('disabled', false);
                    tableCntr = tableChild.DataTable({
                        dom: 'Bfrtip',
                        pageLength: 5,
                        data: data.data, // Los datos deben ser proporcionados aquí
                        columns: [
                            { title: 'Contenedor', data: 'DETCARGAB.IDEN' },
                            { title: 'Bultos', data: 'DETCARGAB.BULTOS' },
                            { title: 'Peso', data: 'DETCARGAB.PESO_ORIG' }
                        ],
                        select: true,
                        language: lang,
                        buttons: [
                            { extend: 'create', editor: editorCntr },
                            { extend: 'edit', editor: editorCntr },
                            { extend: 'remove', editor: editorCntr }
                        ]
                    });

                    // Agregar la tabla a la fila

                })
                .catch(function (error) {
                    console.error('Error:', error);
                    // Puedes manejar el error de otra manera, como mostrar un mensaje de error en la página.
                });

            editorCntr.on('onPreSubmit', function (e, data, action) {
                if (action === 'create') {
                    if (!/^\d+(\.\d+)?$/.test(data.data[0].DETCARGAB.BULTOS)) {
                        var numeroConComa = data.data[0]['PESONETO'];
                        var hola = 2;

                    }
                    else {
                        return 'hola'
                    }


                    console.log('Datos del nuevo registro:', data.data);
                }
            });
        }

        function destroyChild(row) {
            var table2 = $('table', row.child());
            table2.detach();
            table2.DataTable().destroy();

            row.child.hide();


        }

        booking = bl.trim();
        //tableFiles.on('select', function (e, data) {
        //    console.log(this.field('CODIGO'));
        //});

        $('#myModal').modal('show');
        $('#myModal').addClass('center-opening show');
        $('body').addClass('modal-open');

        $('#myModal').on('hidden.bs.modal', function () {
            $(this).removeClass('center-opening show');
            $('body').removeClass('modal-open');
        });



    }

    $(document).ready(function () {
        editor = new DataTable.Editor({
            ajax: {
                create: {
                    type: 'POST',
                    url: urlApi + 'FilesExpo/Create',
                    beforeSend: sf.setModuleHeaders,
                    data: function (data) {
                        data.cn = cn_hash;
                        data.pkey = pkey;
                        data.nituser = nitUser;
                        data.isagent = isAgent;
                        data.Booking = booking;
                        data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                        return data;
                    }
                },
                edit: {
                    type: 'PUT',
                    url: urlApi + 'FilesExpo/Edit',
                    beforeSend: sf.setModuleHeaders,
                    data: function (data) {
                        data.cn = cn_hash;
                        data.pkey = pkey;
                        data.nituser = nitUser;
                        data.isagent = isAgent;
                        data.Booking = booking;
                        data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                        return data;
                    }
                },
                remove: {
                    type: 'POST',
                    url: urlApi + 'FilesExpo/Remove',
                    beforeSend: sf.setModuleHeaders,
                    data: function (data) {
                        data.cn = cn_hash;
                        data.pkey = pkey;
                        data.nituser = nitUser;
                        data.isagent = isAgent;
                        data.Booking = booking;
                        data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                        return data;
                    }
                },
                upload: {
                    type: 'POST',
                    url: urlApi + 'FilesExpo/Upload',
                    beforeSend: sf.setModuleHeaders,
                    data: function (data) {
                        data.cn = cn_hash;
                        data.pkey = pkey;
                        data.nituser = nitUser;
                        data.isagent = isAgent;
                        data.Booking = booking;
                        data.Codigo = editor.field('CODIGO').val();
                        return data;
                    }
                }
            },
            table: '#tbl_files',
            fields: [
                {
                    "label": "BOOKING:",
                    "name": "BOOKING"
                },
                {
                    "label": "Tipo Form:",
                    "name": "CATEGORIA",
                    type: 'select',
                    options: [
                        { label: 'SAE', value: 'SAE' },
                        { label: 'Formulario 1166 ', value: 'F66' },
                        { label: 'FMM - Formulario Movimiento de Mercancia', value: 'FMM' },
                        { label: 'DTA - Declaracion de transito aduanero', value: 'DTA' },
                        { label: 'Memoriales', value: 'MEM' }
                    ],
                    placeholder: 'Selecciona una opción'
                },
                {
                    "label": "CODIGO:",
                    "name": "CODIGO"
                },
                {
                    "label": "ADJUNTAR:",
                    "name": "ADJUNTO",
                    "type": "upload",
                    display: function (file_id) {
                        return `<a href="${editor.file('ARCHIVOS_BOOKINGS', file_id).web_path}">${editor.file('ARCHIVOS_BOOKINGS', file_id).filename}</a>`
                    },
                    clearText: "Eliminar archivo",
                    noImageText: "No hay archivo",
                    error: "No se puede cargar el archivo"
                }
            ],
            i18n: i18
        });

        table = $('#tbl_bookings').DataTable({
            dom: 'Bfrtip',
            serverSide: true,
            processing: true,
            ajax: {
                url: urlApi + 'Bookings/Get',
                beforeSend: sf.setModuleHeaders,
                data: function (data) {
                    data.cn = cn_hash;
                    data.pkey = pkey;
                    data.nituser = nitUser;
                    data.isagent = isAgent;
                    return data;
                },
                type: 'POST'
            },
            columns: [
                {
                    title: "RESERVA",
                    data: "BL_ORIG",
                    className: "dt-head-center"
                },
                {
                    title: "NIT",
                    data: "TER_FACT",
                    className: "dt-head-center"
                },
                {
                    title: "PUERTO",
                    data: "PRT_RCL",
                    className: "dt-head-center"
                },
                {
                    title: "VIAJE",
                    data: "VIAJE",
                    className: "dt-head-center"
                },
                {
                    title: "ESTADO",
                    data: "CGO_DSC",
                    render: function (data, type, row) {
                        let bgColor;

                        switch (data) {
                            case '0':
                                bgColor = "pink";
                                break;
                            case '1':
                                bgColor = "lightblue";
                                break;
                            case '2':
                                bgColor = "lightgreen";
                                break;
                            case '3':
                                bgColor = "yellow";
                                break;
                            default:
                                bgColor = "lightgray";
                                break;
                        }

                        let selectOptions = [
                            { value: 'Pendiente', id: '0' },
                            { value: 'En revisión', id: '1' },
                            { value: 'Completo', id: '2' },
                            { value: 'Incompleto', id: '3' }
                        ];

                        let selectHtml = `<select id="sl_state" onchange="CambiarEstado('${row.BL_ORIG}',this)">`;

                        for (let option of selectOptions) {
                            selectHtml += `<option id="${option.id}" value="${option.value}" ${data === option.value ? 'selected' : ''}>${option.value}</option>`;
                        }

                        selectHtml += `</select>`;

                        return selectHtml;
                    },
                    className: "dt-head-center"
                },
                {
                    title: "OBSERVACIONES",
                    data: "ANEXO",
                    render: function (data, type, row) {
                        var imageUrl = 'images/icon_bulkmail_32px.gif';
                        var imageStyle = 'width: 30px; height: 30px;';
                        return `<button type="button" onclick="VerObservaciones('${row.BL_ORIG}')" style="border: none; background: none; padding: 0; margin: 0;"><img src="${imageUrl}" alt="Observaciones" style="${imageStyle}"></button>`;
                    },
                    className: "dt-head-center"
                },
                {
                    title: "DOCUMENTOS",
                    data: "ADJUNTOS",
                    render: function (data, type, row) {
                        if (type === 'display')
                            return `<button type="button" id="btn-admin" class="btn btn-primary" onclick="leerDocumentos('${row.BL_ORIG}');">Administrar (${data})</button>`;
                    },
                    defaultContent: '',
                    className: "dt-head-center"
                }
            ],
            select: false,
            lengthChange: false,
            responsive: true,
            language: lang,
            buttons: [


            ]

        });
        $('#puertoFilter').on('change', function () {
            var selectedValue = $(this).val();
            // Log a message to the console when the change event is triggered
            console.log('Dropdown change event triggered. Selected Puerto:', selectedValue);
            table.column(2).search(selectedValue).draw();
        });
        table.on('select', function (e, dt, type, indexes) {
            //fila = table.row({ selected: true }).data();
            nitUser = table.row({ selected: true }).data() !== undefined ? table.row({ selected: true }).data().TER_FACT.trim() : nitUser;
            $('.btn-editar').removeClass('disabled');
        });

        editor.on('open', function (e, data) {
            editor.field('BOOKING').val(booking);
            //editor.field('TER_FACT').val(nit.trim());

        });

        $('#myModal').on('hidden.bs.modal', function () {
            table.draw();
        });






        editor.on('preUpload', function (event, field, file) {//preSubmit //preUpload
            //console.log(event);
            //console.log(`Field: ${field}`);
            //console.log(file);
            if (file !== 'remove') {
                var codigo = this.field('CODIGO');
                if (codigo.val().length <= 1) {
                    codigo.error('El campo codigo es requerido para adjuntar el archivo.');
                } else {
                    codigo.error();
                }
                // ... additional validation rules
                // If any error was reported, cancel the submission so it can be corrected
                if (this.inError()) {
                    return false;
                }
            }
        });


    });
</script>
<asp:PlaceHolder ID="phMain" runat="server">
    <div class="container">
         <b>Seleccionar puerto:</b> 
         <th>
         <select id="puertoFilter">
             <option value="BAQ">Barranquilla</option>
             <option value="CTG">Cartagena</option>
             <option value="BUN">Buenaventura</option>
             <option value="SMR">Santa Marta</option>
             <option value="TRB">Turbo</option>
	     <option value="">Todos los Puertos</option>
             <!-- Add more options as needed -->
         </select>
        </th>
        <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered display align-middle text-center" id="tbl_bookings" width="100%">
            <thead>
                <tr>
                   
                </tr>

            </thead>
        </table>
    </div>   
    <div class="modal fade " id="myModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel" >
        <div class="modal-dialog modal-lg modal-vertical-centered modal-dialog-custom modal-lg-custom" role="document">
            <div class="modal-content modal-content-custom">
                <div class="modal-header">
                    <h4 class="modal-title text-center" id="myModalLabel">Archivos adjuntos</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body tabla-contenedor" id="modal-body">
                    <div class="tabla-contenedor-tbl">
                        <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered display align-middle text-center" id="tbl_files" width="100%">
                            <thead>
                            </thead>
                        </table>
                    </div>
                </div>             
            </div>
        </div>
    </div>
</asp:PlaceHolder>

