// Fetching a JSON file
// Returns an array of objects
async function fetchJSONData(path) {
    return fetch(path)
        .then(response => {
            // Check if the response is ok (status in the range 200-299)
            // If not, throw an error
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(text => {
            // Parse the JSON text
            // If parsing fails, catch the error and log it
            try {
                return JSON.parse(text);
            } catch (error) {
                console.error('Error parsing JSON:', error);
            }
        })
        .catch(error => {
            console.error('Failed to load data:', error);
        })
    ;
}

export {
    fetchJSONData
};