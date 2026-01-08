import axios from 'axios';

const API_BASE_URL = '/api/comprobantes';

const comprobanteService = {
    // Obtener lista de comprobantes
    async getAll(params = {}) {
        const response = await axios.get(API_BASE_URL, { params });
        return response.data;
    },

    // Obtener comprobante por ID
    async getById(id) {
        const response = await axios.get(`${API_BASE_URL}/${id}`);
        return response.data;
    },

    // Crear nuevo comprobante
    async create(data) {
        const response = await axios.post(API_BASE_URL, data);
        return response.data;
    },

    // Anular comprobante
    async anular(id) {
        const response = await axios.put(`${API_BASE_URL}/${id}/anular`);
        return response.data;
    }
};

export default comprobanteService;