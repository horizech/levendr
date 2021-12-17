import { LevendrApiProvider } from '../api_providers';

export const levendrService = {
    checkInitialized,
    initialize
};

function checkInitialized() {
    return LevendrApiProvider.checkInitialized();
}

function initialize(userInfo) {
    return LevendrApiProvider.initialize(userInfo);
}
