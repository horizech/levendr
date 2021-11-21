import { LevendrApiProvider } from '../api_providers';

export const levendrService = {
    checkInitialized
};

function checkInitialized() {
    return LevendrApiProvider.checkInitialized();
}
