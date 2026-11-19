$().ready(function () {
    $("#btnModalBuscarSustancia1").click(
        function () {

            var Codigo = $('input[id="Codigo"]').val();
            var Descripcion = $('input[id="Descripcion"]').val();
            var idGrupoFarmacologico = $('select[id="idGrupoFarmacologico"]').val();
            var EsSustanciaControlada = $('select[id="EsSustanciaControlada"]').val();

            var CAS = $('input[id="CAS"]').val();
            var PMCompuesto = $('input[id="PMCompuesto"]').val();
            var FormulaMolecular = $('input[id="FormulaMolecular"]').val();

            var url = $(this).attr('data-actionurl');
            
            if (url.length > 0) {
                
                $('div[id="resultadosBusqueda"]').toggleClass('sk-loading');
                
                $.get(url, { Codigo: Codigo, Descripcion: Descripcion, idGrupoFarmacologico: idGrupoFarmacologico, EsSustanciaControlada: EsSustanciaControlada, CAS: CAS, PMCompuesto: PMCompuesto, FormulaMolecular: FormulaMolecular })
                    .success(
                        function (result) {

                            if (result !== null) {

                                $('div[id="resultadosBusqueda"]').html(result);

                            }
                            else {

                                $("#lblInformacionModal").html("");
                            }

                            $('div[id="resultadosBusqueda"]').toggleClass('sk-loading');

                        }
                    )
                    //.fail(function () {
                    //    debugger
                    //    //$("#lblInformacionModal").html("Error al conectarse");
                    //    $('.ibox-content').toggleClass('sk-loading');
                    //});
                .error(function (jqXHR, textStatus, errorThrown) {
                    $('div[id="resultadosBusqueda"]').toggleClass('sk-loading');
                    //debugger
                });


            } else {
                $("#lblInformacionModal").html("Complete todos los campos");
            }

            return false;

        });

    //Seleccionar sustancias

    $("#btnSelecionarSustancias1").click(
        function () {


            var selected = [];
            $('input[name="Selecciona"').each(function () {
                if ($(this).is(":checked")) {
                    selected.push($(this).val());
                }
            });
            
            var listasustancias = [];

            var tabla = $("#tablaListadoSustancias");


            if ($(tabla).length > 0) {
                if (listasustancias.length == 0) {
                    $(tabla).find("tr").each(function (key, value) {
                        //debugger
                        var sustanciam = FindModel(value, key)
                        if (sustanciam != null) {
                            listasustancias.push(sustanciam);

                        }
                    })
                }
                else {
                    $.each(listasustancias, function (key, value) {
                        SetModel(value, key)
                    });

                }
            }

            var url = $(this).attr('data-actionurl');

            if (url.length > 0) {

                $('#loading').toggleClass('sk-loading');

                $.post(url, $.param({ selecionados: selected, sustancias: listasustancias }, true))
                    .success(
                        function (result) {

                            if (result !== null) {

                                $('div[id="ListaSolicitud"]').html(result);

                            }
                            else {

                                $("#lblInformacionModal").html("");
                            }

                            $('#loading').toggleClass('sk-loading');

                        }
                    )
                    //.fail(function () {
                    //    debugger
                    //    //$("#lblInformacionModal").html("Error al conectarse");
                    //    $('.ibox-content').toggleClass('sk-loading');
                    //});
                .error(function (jqXHR, textStatus, errorThrown) {
                    $('#loading').toggleClass('sk-loading');
                    //debugger
                });


            } else {
                $("#lblInformacionModal").html("Complete todos los campos");
            }

        });



    

});