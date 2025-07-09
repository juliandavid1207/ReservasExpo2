<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="View.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.View" %>
<%--<%@ Register Src="~/desktopmodules/ReservasExportaciones/Components/Controls/Observations.ascx" TagPrefix="uc1" TagName="ControlObservations" %>--%>

<link rel="stylesheet" type="text/css" href="/DesktopModules/ReservasExportaciones/vendor/DataTables/datatables.min.css" />
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
<script type="text/javascript" src="/DesktopModules/ReservasExportaciones/vendor/DataTables/datatables.min.js"></script>
<script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>

<script>   

    const DEBUG = true;
    var moduleScope = $('#<%=phMain.ClientID %>');
    var self = moduleScope;
    const moduleId = <%=ModuleId%>;
    const userID = <%=UserId%>;
    const portalID =<%=PortalId%>;
    const cn_hash = "<%=ConnectionString%>";
    const pkey = "<%=PublicKey%>";
    const isAgent = "<%=IsAgent%>";
    var nitUser = "<%=NitUser%>";
    const sf = $.ServicesFramework(moduleId);
    const urlApi = sf.getServiceRoot('ReservasExportaciones');
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
                    "label": "Booking:",
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
                    "label": "Código:",
                    "name": "CODIGO"
                },
                {
                    "label": "Peso:",
                    "name": "PESO"
                },
                {
                    "label": "Bultos:",
                    "name": "BULTOS"
                },
                {
                    "label": "Comentario:",
                    "name": "COMENTARIO",
                    "type": "textarea"
                },
                {
                    "label": "Adjuntar:",
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
            stateSave : true,
            order: [4, 'desc'], 
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
                    title: "Reserva",
                    data: "BL_ORIG",
                    className: "dt-head-center"
                },
                {
                    title: "Nit",
                    data: "TER_FACT",
                    className: "dt-head-center"
                },
                {
                    title: "Puerto",
                    data: "PRT_RCL",
                    className: "dt-head-center"
                },
                {
                    title: "Viaje",
                    data: "VIAJE",
                    className: "dt-head-center"
                },
                {
                    title: "CutOff",
                    data: "DATE_CUTOFF",
                    className: "dt-head-center"
                },
                {
                    title: "Estado",
                    data: "CGO_DSC",                 
                    className: "dt-head-center"
                },
                {
                    title: "Observaciones",
                    data: "ANEXO",
                    render: function (data, type, row) {
                        var imageUrl = '../images/img_observaciones.gif';
                        var imageStyle = 'width: 30px; height: 30px;';
                        return `<button type="button" onclick="VerObservaciones('${row.BL_ORIG}')" class="btn btn-primary">Comentarios</button>`;
                    },
                    className: "dt-head-center"              
                },
                {
                    title: "Documentos",
                    data: "ADJUNTOS",
                    render: function (data, type, row) {
                        if (type === 'display')
                            return `<button type="button" id="btn-admin" class="btn btn-primary" onclick="leerDocumentos('${row.BL_ORIG}','${row.DATE_CUTOFF}');">Documentos (${data})</button>`;
                    },
                    defaultContent: '',
                    className: "dt-head-center"
                }
            ],
            columnDefs: [
                {
                    targets: 4, // Asegúrate que este sea el índice correcto de la columna DATE_CUTOFF
                    render: function (data, type, row) {
                        if (!data) return '';

                        const [datePart, timePart] = data.split(' ');
                        const [day, month, year] = datePart.split('/');
                        const [hour = 0, minute = 0, second = 0] = timePart ? timePart.split(':') : [0, 0, 0];

                        const dateObj = new Date(
                            parseInt(year),
                            parseInt(month) - 1,
                            parseInt(day),
                            parseInt(hour),
                            parseInt(minute),
                            parseInt(second)
                        );

                        if (type === 'display' || type === 'filter') {
                            return data;
                        }

                        return dateObj.getTime();
                    },
                    createdCell: function (td, cellData, rowData) {
                        const data = rowData.DATE_CUTOFF;
                        if (!data) return;

                        const [datePart, timePart] = data.split(' ');
                        const [day, month, year] = datePart.split('/');
                        const [hour = 0, minute = 0, second = 0] = timePart ? timePart.split(':') : [0, 0, 0];

                        const cutoffDate = new Date(
                            parseInt(year),
                            parseInt(month) - 1,
                            parseInt(day),
                            parseInt(hour),
                            parseInt(minute),
                            parseInt(second)
                        );

                        const now = new Date();
                        const oneDayLater = new Date();
                        oneDayLater.setDate(now.getDate() + 3);                     
                       

                        const isInNextOneDay = cutoffDate >= now && cutoffDate <= oneDayLater;
                        const expiredDate = cutoffDate <= now;                      
                        const estado = rowData.CGO_DSC;     

                        if (estado === "Completo") {
                            $(td).css('background-color', '#4cff33');
                        }
                        else if (estado === "En Revisión") {
                            $(td).css('background-color', '#f9f95c');
                        }                     
                        else if (estado === "Incompleto") {
                            $(td).css('background-color', '#fca286');
                        }                                      
                        
                    }
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
            console.log('Dropdown change event triggered. Selected Puerto:', selectedValue);
            table.column(2).search(selectedValue).draw();
        });
        $('#estadoFilter').on('change', function () {
            var selectedValue = $(this).val();
            console.log('Dropdown change event triggered. Selected estado:', selectedValue);
            table.column(5).search(selectedValue).draw();
        });
        table.on('select', function (e, dt, type, indexes) {          
            nitUser = table.row({ selected: true }).data() !== undefined ? table.row({ selected: true }).data().TER_FACT.trim() : nitUser;
            $('.btn-editar').removeClass('disabled');
        });

        editor.on('open', function (e, data) {
            editor.field('BOOKING').val(booking);
        });

        $('#myModal').on('hidden.bs.modal', function () {
            table.ajax.reload(null, false);
/*            table.draw();*/
        });

        function reloadTablePreservePage() {
            const currentPage = table.page(); 
            table.ajax.reload(function () {
                table.page(currentPage).draw(false); 
            }, false);
        }

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
                <option value="">Todos los Puertos</option>
                <option value="BAQ">Barranquilla</option>
                <option value="CTG">Cartagena</option>
                <option value="BUN">Buenaventura</option>
                <option value="SMR">Santa Marta</option>
                <option value="TRB">Turbo</option>
            </select>
        </th>
        <b>Seleccionar estado:</b>
        <th>
            <select id="estadoFilter">
                <option value="">Todos los Estados</option>
                <option value="Pendiente">Sin Documentos</option>
                <option value="En revisión">En Revisión</option>
                <option value="Completo">Completo</option>
                <option value="Incompleto">Incompleto</option>               
            </select>
        </th>
        <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered display align-middle text-center" id="tbl_bookings" width="100%">
            <thead>
                <tr>
                   
                </tr>
            </thead>
        </table>
    </div>   
</asp:PlaceHolder>
 <div class="modal fade contenedorModal" id="solicitarModal" tabindex="-1" role="dialog" aria-labelledby="modalLabel" aria-hidden="true">
     <div class="modal-dialog" role="document">
         <div class="modal-content">
             <div class="modal-header">
                 <h5 class="modal-title" id="modalLabel"><i class="bi bi-folder-check"></i>Actualizar estado</h5>
                 <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                     <span aria-hidden="true">&times;</span>
                 </button>
             </div>
             <div class="modal-body" id="modalBodyContent">
                 <!-- Contenido del modal dinámico -->
             </div>
             <div class="modal-footer">
                 <button type="button" class="btn btn-primary" id="enviarEstado">Cambiar estado</button>
                 <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
             </div>
         </div>
     </div>
 </div>


