// Formatear moneda en soles peruanos
export const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-PE', {
        style: 'currency',
        currency: 'PEN'
    }).format(amount);
};

// Formatear fecha
export const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-PE', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
};

// Formatear número de comprobante
export const formatNumeroComprobante = (serie, numero) => {
    return `${serie}-${String(numero).padStart(8, '0')}`;
};