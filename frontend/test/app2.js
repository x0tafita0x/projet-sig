var app = angular.module('myApp', []);
app.controller('MainController',function($scope,$http){
    $scope.ecoles = [];
    $scope.categorie  = [];
    $scope.info = [];
    $scope.showInsert = false;
    $scope.init = function(){
        console.log("initialisation");
        var mapOptions = {
            center : new google.maps.LatLng(-18.986021,47.532735),
            zoom:18
        };
        var carte = new google.maps.Map(document.getElementById("carteid"),mapOptions);
        $scope.carte = carte;
        google.maps.event.addListener($scope.carte,"click",function(event){
            $scope.$apply(function() {
                $scope.showInsert = true;
            });    

        });
    }


    $scope.init();
});
