(function () {

    var app = angular.module("plunker", ["ui.bootstrap", 'angular-flot']);

    app.controller("MainCtrl", function ($scope, $http, $interval) {
        var GuidString;
        var delay;
        var delayTotalTime;

        var promise;// store the interval promise in this variable
        var promiseTotalTime;

        var counterTime;

        function initializationData() {
            $scope.data = { visible: false };
            $scope.dataXML = { visible: false };
            $scope.dataHTML = { visible: false };
            $scope.dataset = [[[10, 1], [20, 3], [35, 1], [40, 1], [50, 8], [60, 1], [70, 1]]];
            delay = 3000;
            delayTotalTime = 1000;
            $scope.totalTime = "0";
            $scope.averageSpeed = "0";
        }

        initializationData();

        $scope.start = function () {// starts the interval
            counterTime = 0;
            $scope.stop();// stops any running interval to avoid two intervals running at the same time
            promise          = $interval($scope.getArrayForFlot, delay);// store the interval promise
            promiseTotalTime = $interval($scope.myTimer, delayTotalTime);
        };
        $scope.myTimer = function () {
            counterTime++;
            var dateMy = new Date();
            dateMy.setHours(0, 0, counterTime, 0);

            $scope.totalTime = dateMy.toString().split(" ")[4];
            var temp = parseInt($scope.foundPages / counterTime,10);
            //alert("temp: " + temp + "\n" + "$scope.foundPages: " + $scope.foundPages + "\n" + "$scope.counterTime: " + counterTime);
            $scope.averageSpeed = parseInt($scope.foundPages / counterTime, 10);
        }

        $scope.stop = function () {// stops the interval
            $interval.cancel(promise);
            $interval.cancel(promiseTotalTime);

        };

        $scope.$on('$destroy', function () {
            // stops the interval when the scope is destroyed, this usually happens when a route is changed and 
            // the ItemsController $scope gets destroyed. The destruction of the ItemsController scope does not
            // guarantee the stopping of any intervals, you must be responsible for stopping it when the scope is
            // is destroyed.
            $scope.stop();
        });




        $scope.getHistory = function () {
            $http({
                method: "GET",
                url: "/api/LookForSiteAddress/",
            }).then(function mySucces(response) {
                $scope.dataXML = { visible: false };
                $scope.dataHTML = { visible: false };

                $scope.allLogs = response.data;
                $scope.totalAddresses = $scope.allLogs.length;
                $scope.currentPage = 1;
                $scope.itemsPerPage = 5;
                $scope.maxSize = 6;

                $scope.show();

                $scope.data = { visible: true };

            }, function myError(response) {
                alert(response.statusText);
            });

        }
        $scope.getXmlHistory = function (Id) {

            $http({
                method: 'DELETE',
                url: "/api/LookForSiteAddress/" + Id
            }).then(function mySucces(response) {

                $scope.dataHTML = { visible: false };



                $scope.allLogsXML = response.data;
                $scope.totalAddressesXML = $scope.allLogsXML.length;
                $scope.currentPageXML = 1;
                $scope.itemsPerPageXML = 5;
                $scope.maxSizeXML = 6;

                $scope.showXML();

                $scope.dataXML = { visible: true };


            }, function myError(response) {
                alert(response.statusText);
            });

        }
        $scope.getHtmlHistory = function (Id) {

            $http.put("/api/LookForSiteAddress/" + Id, { 'UrlAddress': 'yo', 'GuidString': 'Yo', 'Id': 0 })
                                    .then(function mySucces(response) {
                                        var temp = response.data;

                                        $scope.allLogsHTML = response.data;
                                        $scope.totalAddressesHTML = $scope.allLogsHTML.length;
                                        $scope.currentPageHTML = 1;
                                        $scope.itemsPerPageHTML = 5;
                                        $scope.maxSizeHTML = 6;

                                        $scope.showHTML();

                                        $scope.dataHTML = { visible: true };

                                    }, function myError(response) {
                                        alert(response.statusText);
                                    });
        }

        $scope.getArrayForFlot = function () {
            $http({
                method: "GET",
                url: "/api/LookForSiteAddress/?guid=" + GuidString
            }).then(function mySucces(response) {
                var dataFromServer = [response.data];
                if (dataFromServer.length > 0) {
                    $scope.dataset = dataFromServer;
                    var sd = dataFromServer[0];//dataFromServer.length - 1
                    $scope.foundPages = sd[sd.length-1][0];
                }

            }, function myError(response) {
                alert(response.statusText);
            });

        }

        $scope.getPing = function (Target) {

            if (Target.length > 6) {
                $http.post("/api/Ping/", { 'UrlAddress': Target, 'GuidString': 'Yo', 'Id': 0 })
                        .then(function mySucces(response) {
                            var temp = response.data;
                            var str = "";
                            if (temp.length > 2) {
                                for (var i = 0; i < temp.length; i++) {
                                    if (i % 2 == 0)
                                        str += temp[i] + "  " + temp[i + 1];
                                }
                                $scope.Results = str;
                            }
                            else {
                                $scope.Results = temp;
                            }

                        }, function myError(response) {
                            alert(response.statusText);
                        });
            }
            else {
                alert("Enter a target address");
            }
        }

        $scope.findSitemap = function (Target) {

            if (Target.length > 6) {
                $scope.somestyle = { background: 'Red', color: 'Blue' };
                $scope.Results = "Wait please ..."
                $scope.foundPages = 0;
                $scope.averageSpeed = 0;



                GuidString = guid();

                $scope.start();// starting the interval by default



                $http.post("/api/LookForSiteAddress/", { 'UrlAddress': Target, 'GuidString': GuidString, 'Id': 0 })
                        .then(function mySucces(response) {

                            $scope.stop();

                            var temp = response.data;

                            var str = "";
                            if (temp.length > 2) {
                                for (var i = 0; i < temp.length; i++) {
                                    if (i % 2 == 0)
                                        str += temp[i] + "  " + temp[i + 1];
                                }
                                $scope.Results = str;
                            }
                            else {
                                $scope.Results = temp;
                            }

                        }, function myError(response) {
                            $scope.stop();

                            alert(response.statusText);
                        });
            }
            else {
                alert("Enter a target address");
            }
        }

        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                    .toString(16)
                    .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                s4() + '-' + s4() + s4() + s4();
        }

        function ajaxload() {
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange =
                function () {
                    if (this.readyState == 4) {
                        if (this.status >= 200 && xhr.status < 300) {
                            var resp = JSON.parse(this.responseText);
                            var all_data = [{ color: 4, data: resp }];
                            $.plot($("#placeholder"), all_data);
                            $(".percentage").html("found " + resp[resp.length - 1][0] + " pages");
                        }
                    }
                }
            xhr.open('GET', '/Home/JsonForFlotcharts?guid=' + GuidString);
            xhr.send();
        }


        $scope.show = function () {
            $scope.$watch("currentPage", function () {
                setPagingData($scope.currentPage);
            });
        }
        function setPagingData(page) {
            $scope.Logs = $scope.allLogs.slice(
                (page - 1) * $scope.itemsPerPage,
                page * $scope.itemsPerPage
            );
        }
        $scope.showHTML = function () {
            $scope.$watch("currentPageHTML", function () {
                setPagingDataHTML($scope.currentPageHTML);
            });
        }
        function setPagingDataHTML(page) {
            $scope.LogsHTML = $scope.allLogsHTML.slice(
                (page - 1) * $scope.itemsPerPageHTML,
                page * $scope.itemsPerPageHTML
            );
        }

        $scope.showXML = function () {
            $scope.$watch("currentPageXML", function () {
                setPagingDataXML($scope.currentPageXML);
            });
        }


        function setPagingDataXML(page) {
            $scope.LogsXML = $scope.allLogsXML.slice(
                (page - 1) * $scope.itemsPerPageXML,
                page * $scope.itemsPerPageXML
            );
        }

    });


})();