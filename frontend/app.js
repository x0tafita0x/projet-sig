var app = angular.module('myApp', []);
app.controller('MainController',function($scope,$http,$q){
    $scope.ecoles = [];
    $scope.ecoleTarget;
    $scope.title = "Carte des écoles";
    $scope.categorie  = [];
    $scope.categorieActu = [];
    $scope.info = [];
    var mapClickedDeferred;
    $scope.message_config = "Pas encore de zone de recherche";
    $scope.showInsert = false;
    $scope.latRe;
    $scope.longRe;
    $scope.showInfo = false;
    $scope.showUpdate = false;
    $scope.lat = 0;
    $scope.long = 10;
    $scope.carte;
    $scope.last_id = 0;

    //initialisation
    $scope.init = function(){
        console.log("initialisation");
        var mapOptions = {
            center : new google.maps.LatLng(-18.986021,47.532735),
            zoom:18
        };
        $scope.carte = new google.maps.Map(document.getElementById("carteid"),mapOptions);
        $scope.get_all_categorie();
        $scope.get_all_ecole();
        google.maps.event.addListener($scope.carte,"click",function(event){
            $scope.$apply(function() {  
                if (mapClickedDeferred) {
                    mapClickedDeferred.resolve(event.latLng);
                    mapClickedDeferred = null;
                    return;
                }
                if($scope.showInsert || $scope.showUpdate){
                    $scope.showInsert = false;
                    $scope.showUpdate = false;
                    return;
                }
                $scope.lat = event.latLng.lat();
                $scope.long = event.latLng.lng();
                $scope.showInsert = true;
                console.log($scope.categorie);
            });

        });
    }

    //traitement de données
    $scope.get_all_ecole = function(){
        var url = "https://localhost:7009/ecole_api/readAllEco";
        const option = {
            method: 'POST',
            headers: {'Content-Type': 'application/json'}
        };
        fetch(url,option).then(function(response){
            return response.json();
        }).then(function(data){
            if(data){
                $scope.ecoles=data.value;  
                $scope.display_ecole();
            }
        }, function(error){
            console.log(error);
        });
    }
    $scope.get_all_categorie = function(){
        var url = "https://localhost:7009/ecole_api/readAllCat";
        const option = {
            method: 'POST',
            headers: {'Content-Type': 'application/json'}
        };
        fetch(url,option).then(function(response){
            return response.json();
        }).then(function(data){
            if(data){
                $scope.$apply(function(){
                    for(var i = 0; i<data.value.length; i++){
                        $scope.categorie.push({id:data.value[i].id,name:data.value[i].name,selected:false});
                    }
                });
                
            }
        }, function(error){
            console.log(error);
        });
    }
    $scope.insert_ecole = function(n,type){
        $scope.list = $scope.categorie.filter(function(cat){
            return cat.selected;
        })
        var categoList = [];
        for(var i = 0; i<$scope.list.length; i++){
            categoList.push({id:$scope.list[i].id,name:$scope.list[i].name});
        }
        var data = {
            nom:n,
            typeEtablissement:type, 
            longitude:$scope.long,
            latitude:$scope.lat,
            categorie:categoList        
        };
        $http.post("https://localhost:7009/ecole_api/create",JSON.stringify(data),{
            headers: {
              'Content-Type': 'application/json'
            }
          }).then(function(response){
            $scope.create_point(n,type,categoList);
        }, function(error){
            alert("Erreur:",error);
        })
    }
    $scope.get_last_id = function(){
        return $http.get("https://localhost:7009/ecole_api/lastId").then(function(response){
            $scope.last_id = response.data;
        },function(error){
            alert("Erreur:",error);
        })
    }
    $scope.update_ecole = function(n,type){
        $scope.list = $scope.categorieActu.filter(function(cat){
            return cat.selected;
        });

        var categoList = [];
        for(var i = 0; i<$scope.list.length; i++){
            categoList.push({id:$scope.list[i].id,name:$scope.list[i].name});
        }
        console.log($scope.list);
        var data = {
            nom:n,
            typeEtablissement:type,
            categorie:categoList,
            longitude:$scope.ecoleTarget.emplacement.longitude,
            latitude:$scope.ecoleTarget.emplacement.latitude,
            id: $scope.ecoleTarget.id   
        };
        $http.post("https://localhost:7009/ecole_api/update",JSON.stringify(data),{
            headers: {
              'Content-Type': 'application/json'
            }
          }).then(function(response){
                $scope.ecoleTarget.nom = n;
                $scope.ecoleTarget.typeEtablissement = type;
                $scope.ecoleTarget.categorieEcole = categoList;
                $scope.showUpdate = false;
        }, function(error){
            alert("Erreur:",error);
        })
    }
    $scope.delete = function(){
        var id = $scope.ecoleTarget.id;
        console.log($scope.ecoleTarget.id+"---"+id);
        var data = id;
        $http.post("https://localhost:7009/ecole_api/delete",data).then(function(response){
            $scope.ecoleTarget.point.setMap(null);
            $scope.showUpdate = false;
        }, function(error){
            alert("Erreur:",error);
        })
    }
    $scope.search = function(){
        console.log("search");
        $scope.list = $scope.categorie.filter(function(cat){
            return cat.selected;
        });
        if($scope.list.length == 0){
            $scope.get_all_ecole();
            return;
        }
        var categoList = [];
        for(var i = 0; i<$scope.list.length; i++){
            categoList.push({id:$scope.list[i].id,name:$scope.list[i].name});
        }
        var data = {
            categorie:categoList        
        };
        $http.post("https://localhost:7009/ecole_api/read",JSON.stringify(data),{
            headers: {
              'Content-Type': 'application/json'
            }
          }).then(function(response){
            $scope.hide_all_ecole();
            $scope.ecoles = response.data.value;
            $scope.display_ecole();
        }, function(error){
            alert("Erreur:",error);
        })
    }
    $scope.search_by_proxy = function(range){
        console.log("search");
        var data = {
            longitude:$scope.longRe,
            latitude:$scope.latRe,
            range:range       
        };
        $http.post("https://localhost:7009/ecole_api/read",JSON.stringify(data),{
            headers: {
              'Content-Type': 'application/json'
            }
          }).then(function(response){
            $scope.hide_all_ecole();
            $scope.ecoles = response.data.value;
            $scope.display_ecole();
        }, function(error){
            alert("Erreur:",error);
        })
    }

    //autre traitement
    $scope.create_point = function(n,type,cats){
        $scope.get_last_id().then(function(){
            console.log($scope.carte);
            var location = new google.maps.LatLng($scope.lat,$scope.long);
            var mark = new google.maps.Marker({
                position:location,
                map:$scope.carte
            });
            var emplacement = {
                longitude : $scope.lat,
                latitude: $scope.long
            }
            $scope.ecoles.push({
                id:$scope.last_id,
                nom:n,
                typeEtablissement:type,
                categorieEcole:cats,
                point: mark,
                emplacement:emplacement
            });
            var inf = {
                id:$scope.last_id,
                nom:n,
                point:mark
            }
            $scope.info.push(inf);
            google.maps.event.addListener(mark,"mouseover",function(event){
                $scope.$apply(function() {
                    if($scope.showUpdate || $scope.showInsert){
                        return;
                    }
                    $scope.ecoleTarget = $scope.ecoles[$scope.ecoles.length - 1];
                    $scope.showInfo = true;
                });    
            });
            google.maps.event.addListener(mark,"click",function(event){
                $scope.$apply(function(){
                    $scope.showInfo = false;
                    $scope.ecoleTarget = $scope.ecoles[$scope.ecoles.length - 1];
                    $scope.typeEtablissement = $scope.ecoleTarget.typeEtablissement;
                    $scope.nom = $scope.ecoleTarget.nom;
                    $scope.set_categorie();
                    $scope.showUpdate = true;
                });
            });
            google.maps.event.addListener(mark,"mouseout",function(event){
                $scope.$apply(function(){
                    $scope.showInfo = false;
                });
                
            });
        });
        
        
        // google.maps.event.addListener()
        $scope.showInsert = false;
    }
    $scope.set_categorie = function(){
        $scope.categorieActu = [];
        for(var i = 0; i<$scope.categorie.length; i++){
            $scope.categorieActu.push({id:$scope.categorie[i].id,name:$scope.categorie[i].name,selected:false});
            for(var j = 0; j<$scope.ecoleTarget.categorieEcole.length; j++){
                if($scope.categorie[i].id == $scope.ecoleTarget.categorieEcole[j].id){
                    $scope.categorieActu[i].selected = true;
                    break;
                }
            }
        }
        console.log($scope.categorieActu);
    }
    $scope.waitForMapClick = function() {
        $scope.message_config = "En attente de clique";
        mapClickedDeferred = $q.defer();

        mapClickedDeferred.promise.then(function(latLng) {
            $scope.latRe = latLng.lat();
            $scope.longRe = latLng.lng();
            $scope.message_config = "Longitude: "+$scope.longRe+"\n Latitude: "+$scope.latRe;
        });
    };


    //affichage
    $scope.display_ecole = function(){
        $scope.info = [];
        var passage = [];
        var repet = false;
        for(var i = 0; i<$scope.ecoles.length;i++){
            (function(i){
                for(var j = 0; j<passage.length;j++){
                    if(passage[j] == $scope.ecoles[i].id){
                        repet = true; 
                        break;
                    }
                }
                if(!repet){
                    passage.push($scope.ecoles[i].id);
                    var long = $scope.ecoles[i].emplacement.longitude;
                    var lat = $scope.ecoles[i].emplacement.latitude;
                    var point = new google.maps.LatLng(long,lat);
                    var p = new google.maps.Marker({
                        position: point,
                        map: $scope.carte,
                        draggable:true
                    });
                    $scope.ecoles[i].point = p;
                    $scope.info.push({
                            id: $scope.ecoles[i].id,
                            nom: $scope.ecoles[i].nom,
                            point: p
                        }
                    );
                    (function(info){
                        google.maps.event.addListener(p,"mouseover",function(event){
                            $scope.$apply(function() {
                                if($scope.showUpdate || $scope.showInsert){
                                    return;
                                }
                                $scope.ecoleTarget = $scope.ecoles[i];
                                $scope.showInfo = true;
                            });    
                            
                        });
                        google.maps.event.addListener(p,"click",function(event){
                            $scope.$apply(function(){
                                $scope.showInfo = false;
                                $scope.ecoleTarget = $scope.ecoles[i];
                                $scope.typeEtablissement = $scope.ecoleTarget.typeEtablissement;
                                $scope.nom = $scope.ecoleTarget.nom;
                                $scope.set_categorie();
                                $scope.showUpdate = true;
                            });
                        });
                        google.maps.event.addListener(p,"mouseout",function(event){
                            $scope.$apply(function() {
                                $scope.ecoleTarget = $scope.ecoles[i];
                                $scope.showInfo = false;
                            });  
                        });
                    })($scope.info[$scope.info.length - 1]);
                    
                }
                repet = false;
            })(i);
        }
    }
    $scope.hide_all_ecole = function(){
        console.log($scope.info);
        for(var i = 0; i<$scope.info.length; i++){
            $scope.info[i].point.setMap(null);
        }
    }

    $scope.init();
});
