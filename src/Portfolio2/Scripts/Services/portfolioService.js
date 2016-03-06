(function () {
    'use strict';

    angular
        .module('app')
        .factory('portfolioService', portfolioService);

    portfolioService.$inject = ['$http'];

    function portfolioService($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() {
            return $http.get('/api/portfolio');
        }

    }

    /*
    portfolioService.$inject = ['$resource'];

    function portfolioService($resource) {
        return $resource('/api/portfolio/', {}, {
            query: { method: 'GET', params: {}, isArray: true }
        });
    }
    */
})();