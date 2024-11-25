document.addEventListener("DOMContentLoaded", function () {
    const formClima = document.getElementById("formClima");//referencia al formulario donde se ingresan los datos del clima
    const cardClima = document.getElementById("cardClima");//referencia al contenedor donde se mostrará la información del clima
    const cardTools = document.getElementById("cardTools");//referencia al contenedor de herramientas del card
    const cardTitle = cardClima.querySelector(".card-title");//referencia al titulo del card del clima

    //construye la URL base del controlador en el servidor
    const apiBaseUrl = window.location.protocol + "//" + window.location.host + "/Home/GetWeatherData";

    //agrega un evento al formulario que se dispara cuando el usuario hace submit
    formClima.addEventListener("submit", async function (event) {
        event.preventDefault(); //evita que el formulario recargue la pagina

        //obtiene los valores ingresados por el usuario y elimina espacios innecesarios
        const ciudad = document.getElementById("ciudad").value.trim();
        const countryCode = document.getElementById("countryCode").value.trim();

        //verifica que el usuario haya ingresado la ciudad
        if (!ciudad) {
            alert("Por favor, ingrese el nombre de la ciudad.");
            return;
        }

        //verifica que el usuario haya ingresado el codigo de pais
        if (!countryCode) {
            alert("Por favor, ingrese el código del país según la norma ISO 3166-1 alpha-2");
            return;
        }

        try {
            //realiza una consulta a la API para obtener los datos del clima
            const response = await fetch(`${apiBaseUrl}?city=${ciudad}&countryCode=${countryCode}`);

            //si la respuesta no es exitosa
            if (!response.ok) {
                const errorData = await response.json();
                alert(`Error del servidor: ${errorData.message || "Error desconocido."}`);
                return;
            }

            //obtiene los datos en formato JSON
            const climaDataArray = await response.json();
            console.log(climaDataArray);

            //accede al primer elemento del arreglo
            const climaData = climaDataArray[0];

            //limpia cualquier contenido previo dentro del cuerpo del card
            const cardBody = cardClima.querySelector(".card-body");
            cardBody.innerHTML = "";

            //crea el contenido HTML dinamico con los datos del clima
            const contenidoHTML = `
            <div class="card shadow-sm text-center">
                <div class="card-body">
                    <!-- Icono del clima -->
                    <div class="mb-3">
                        <img src="weather-icons-master/production/fill/openweathermap/${climaData.imagen}.svg" 
                             alt="Icono del clima" 
                             class="img-fluid" 
                             style="width: 50px; height: 50px;">
                    </div>

                    <!-- Temperatura -->
                    <h2 class="text-primary mb-2">${climaData.temperatura}</h2>

                    <!-- Descripción -->
                    <h5 class="text-secondary mb-3">${climaData.climaDescripcion}</h5>

                    <!-- Nubosidad y Humedad -->
                    <div class="d-flex justify-content-center gap-3 mb-3">
                        <div>
                            <i class="meteocons-cloud" style="font-size: 20px;"></i>
                            <span>Nubosidad: ${climaData.porcentajeNubes}</span>
                        </div>
                        <div>
                            <i class="meteocons-droplet" style="font-size: 20px;"></i>
                            <span>Humedad: ${climaData.porcentajeHumedad}</span>
                        </div>
                    </div>

                    <!-- Horas de amanecer y anochecer -->
                    <div>
                        <p class="mb-1">
                            <i class="meteocons-sunrise" style="font-size: 20px;"></i> 
                            Amanecer: ${climaData.amanecer}
                        </p>
                        <p>
                            <i class="meteocons-sunset" style="font-size: 20px;"></i> 
                            Puesta de sol: ${climaData.anocher}
                        </p>
                    </div>
                </div>
            </div>`;

            //inserta el contenido dinámico en el card
            cardBody.innerHTML = contenidoHTML;

            //muestra el card y habilita las herramientas visuales
            cardClima.querySelector(".card-body").style.display = "block";
            cardTools.style.display = "block";

            //cctualiza el titulo del card con la ciudad seleccionada
            cardTitle.innerHTML = `<i class="fas fa-cloud"></i> Resultados del Clima: ${climaData.ciudad} - ${climaData.pais}`;

            //si la tarjeta esta contraida
            if (cardClima.classList.contains("collapsed-card")) {
                cardClima.classList.remove("collapsed-card"); //expande la tarjeta
                cardClima.querySelector(".card-tools .btn-tool i").classList.replace("fa-plus", "fa-minus");
            }
        } catch (error) {
            //muestra un mensaje de error si algo falla durante la consulta
            alert(`Error al obtener el clima: ${error.message}`);
        }
    });
});