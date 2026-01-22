function initialize() {
    var mapOptions = {
        center: new google.maps.LatLng(-18.397, 45.644),  // donner le point de centrage de la map
        zoom: 6 // zoom de la map a louverture
    };
    var carte = new google.maps.Map(document.getElementById("carteid"),
        mapOptions); // prendre l id de la balise ou doit etre le dessin de la map
    var location = new google.maps.LatLng(-18.397, 45.644); // un point de la map 
    var marker = new google.maps.Marker({ // option attribuer au point location 
        position: location, //le point location
        draggable: true, // on peut le deplacer
        map: carte // la carte ou elle est
    });

    var ligne = [ // les point par lequel la ligne qui relie les deux point passe
        new google.maps.LatLng(-18.397, 45.644),
        new google.maps.LatLng(-17.780, 48.222),
        new google.maps.LatLng(-17.580, 48.322),

    ];

    var traceLigne = new google.maps.Polyline({
        path: ligne,//chemin du tracé
        strokeColor: "#FF0000",//couleur du tracé
        strokeOpacity: 1.0,//opacité du tracé
        strokeWeight: 2//grosseur du tracé
    });

    traceLigne.setMap(carte); // tracer la ligne sur la map

}
var div = document.getElementById('carteid');
google.maps.event.addDomListener(window, 'load', initialize); //appeler la focntion initialize pour charger la carte sur la page
// google.maps.event.addEventListener(window, 'load', initialize); //appeler la focntion initialize pour charger la carte sur la page
