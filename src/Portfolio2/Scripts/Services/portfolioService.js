(function () {
    'use strict';

    angular
        .module('app')
        .factory('portfolioService', portfolioService);

    portfolioService.$inject = ['$resource'];

    function portfolioService($resource) {
        return $resource('/api/portfolio/', {}, {
            query: { method: 'GET', params: {}, isArray: true }
        });
    }
})();