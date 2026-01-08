import React, { useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import comprobanteService from '../services/comprobanteService';
import { formatCurrency } from '../utils/formatters';
import '../styles/ComprobanteForm.css';

const ComprobanteForm = ({ onSuccess, onCancel }) => {
    const [formData, setFormData] = useState({
        tipo: 'Factura',
        serie: 'F001',
        rucEmisor: '',
        razonSocialEmisor: '',
        rucReceptor: '',
        razonSocialReceptor: '',
        items: [
            { descripcion: '', cantidad: 1, precioUnitario: 0 }
        ]
    });

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const [submitError, setSubmitError] = useState(null);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value,
            // Cuando cambia el tipo, actualizar la serie automáticamente
            ...(name === 'tipo' && {
                serie: value === 'Factura' ? 'F001' : 'B001'
            })
        }));
        // Limpiar errores del campo
        if (errors[name]) {
            setErrors(prev => ({ ...prev, [name]: null }));
        }
    };

    const handleItemChange = (index, field, value) => {
        const newItems = [...formData.items];
        newItems[index] = {
            ...newItems[index],
            [field]: field === 'descripcion' ? value : parseFloat(value) || 0
        };
        setFormData(prev => ({ ...prev, items: newItems }));
    };

    const addItem = () => {
        setFormData(prev => ({
            ...prev,
            items: [...prev.items, { descripcion: '', cantidad: 1, precioUnitario: 0 }]
        }));
    };

    const removeItem = (index) => {
        if (formData.items.length === 1) {
            alert('Debe haber al menos un item');
            return;
        }
        const newItems = formData.items.filter((_, i) => i !== index);
        setFormData(prev => ({ ...prev, items: newItems }));
    };

    const calculateItemSubtotal = (item) => {
        return item.cantidad * item.precioUnitario;
    };

    const calculateSubtotal = () => {
        return formData.items.reduce((sum, item) => sum + calculateItemSubtotal(item), 0);
    };

    const calculateIGV = () => {
        return calculateSubtotal() * 0.18;
    };

    const calculateTotal = () => {
        return calculateSubtotal() + calculateIGV();
    };

    const validateForm = () => {
        const newErrors = {};

        if (!formData.rucEmisor || formData.rucEmisor.length !== 11) {
            newErrors.rucEmisor = 'El RUC debe tener 11 dígitos';
        }

        if (!formData.razonSocialEmisor) {
            newErrors.razonSocialEmisor = 'La razón social es obligatoria';
        }

        if (formData.tipo === 'Factura') {
            if (!formData.rucReceptor || formData.rucReceptor.length !== 11) {
                newErrors.rucReceptor = 'El RUC del receptor es obligatorio para Facturas';
            }
            if (!formData.razonSocialReceptor) {
                newErrors.razonSocialReceptor = 'La razón social del receptor es obligatoria para Facturas';
            }
        }

        formData.items.forEach((item, index) => {
            if (!item.descripcion) {
                newErrors[`item_${index}_descripcion`] = 'La descripción es obligatoria';
            }
            if (item.cantidad <= 0) {
                newErrors[`item_${index}_cantidad`] = 'La cantidad debe ser mayor a 0';
            }
            if (item.precioUnitario <= 0) {
                newErrors[`item_${index}_precioUnitario`] = 'El precio debe ser mayor a 0';
            }
        });

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateForm()) {
            alert('Por favor, corrija los errores en el formulario');
            return;
        }

        setLoading(true);
        setSubmitError(null);

        try {
            const dataToSend = {
                tipo: formData.tipo,
                serie: formData.serie,
                rucEmisor: formData.rucEmisor,
                razonSocialEmisor: formData.razonSocialEmisor,
                ...(formData.tipo === 'Factura' && {
                    rucReceptor: formData.rucReceptor,
                    razonSocialReceptor: formData.razonSocialReceptor
                }),
                ...(formData.tipo === 'Boleta' && formData.rucReceptor && {
                    rucReceptor: formData.rucReceptor,
                    razonSocialReceptor: formData.razonSocialReceptor
                }),
                items: formData.items.map(item => ({
                    descripcion: item.descripcion,
                    cantidad: item.cantidad,
                    precioUnitario: item.precioUnitario
                }))
            };

            const result = await comprobanteService.create(dataToSend);
            alert(`Comprobante creado exitosamente: ${result.serie}-${result.numero}`);
            onSuccess();
        } catch (err) {
            console.error('Error al crear comprobante:', err);

            let errorMessage = 'Error desconocido al crear el comprobante';

            if (err.response && err.response.data) {
                const data = err.response.data;

                // Si es un error de validación con errores estructurados
                if (data.errors && typeof data.errors === 'object') {
                    const errorsList = Object.entries(data.errors)
                        .map(([field, messages]) => {
                            const fieldName = field.charAt(0).toUpperCase() + field.slice(1);
                            const errorMessages = Array.isArray(messages) ? messages.join(', ') : messages;
                            return `• ${fieldName}: ${errorMessages}`;
                        })
                        .join('\n');

                    errorMessage = `Errores de validación:\n\n${errorsList}`;
                }
                // Si es un error con mensaje detail
                else if (data.detail) {
                    errorMessage = data.detail;
                }
                // Si es un error con mensaje simple
                else if (data.message) {
                    errorMessage = data.message;
                }
            } else if (err.message) {
                errorMessage = err.message;
            }

            setSubmitError(errorMessage);

            // Scroll al top para mostrar el error
            window.scrollTo({ top: 0, behavior: 'smooth' });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="comprobante-form">
            <div className="form-header">
                <h2>Crear Nuevo Comprobante</h2>
                <p>Complete todos los campos obligatorios (*)</p>
            </div>

            {submitError && (
                <div className="alert alert-error">
                    <strong>Error:</strong> {submitError}
                </div>
            )}

            <form onSubmit={handleSubmit}>
                {/* Información del Comprobante */}
                <div className="form-card">
                    <h3 className="form-section-title">Información del Comprobante</h3>

                    <div className="form-row">
                        <div className="form-group">
                            <label className="required">Tipo de Comprobante</label>
                            <select
                                name="tipo"
                                value={formData.tipo}
                                onChange={handleChange}
                                required
                            >
                                <option value="Factura">Factura</option>
                                <option value="Boleta">Boleta</option>
                            </select>
                        </div>

                        <div className="form-group">
                            <label className="required">Serie</label>
                            <input
                                type="text"
                                name="serie"
                                value={formData.serie}
                                onChange={handleChange}
                                placeholder="F001 o B001"
                                maxLength="4"
                                required
                            />
                            <small>Formato: F### para Facturas, B### para Boletas</small>
                        </div>
                    </div>
                </div>

                {/* Información del Emisor */}
                <div className="form-card">
                    <h3 className="form-section-title">Información del Emisor</h3>

                    <div className="form-row">
                        <div className="form-group">
                            <label className="required">RUC Emisor</label>
                            <input
                                type="text"
                                name="rucEmisor"
                                value={formData.rucEmisor}
                                onChange={handleChange}
                                placeholder="11 dígitos"
                                maxLength="11"
                                className={errors.rucEmisor ? 'error' : ''}
                                required
                            />
                            {errors.rucEmisor && <div className="error-message">{errors.rucEmisor}</div>}
                        </div>

                        <div className="form-group">
                            <label className="required">Razón Social Emisor</label>
                            <input
                                type="text"
                                name="razonSocialEmisor"
                                value={formData.razonSocialEmisor}
                                onChange={handleChange}
                                placeholder="Nombre de la empresa"
                                className={errors.razonSocialEmisor ? 'error' : ''}
                                required
                            />
                            {errors.razonSocialEmisor && <div className="error-message">{errors.razonSocialEmisor}</div>}
                        </div>
                    </div>
                </div>

                {/* Información del Receptor */}
                <div className="form-card">
                    <h3 className="form-section-title">
                        Información del Receptor {formData.tipo === 'Factura' && '*'}
                    </h3>

                    <div className="form-row">
                        <div className="form-group">
                            <label className={formData.tipo === 'Factura' ? 'required' : ''}>RUC Receptor</label>
                            <input
                                type="text"
                                name="rucReceptor"
                                value={formData.rucReceptor}
                                onChange={handleChange}
                                placeholder="11 dígitos (opcional para Boletas)"
                                maxLength="11"
                                className={errors.rucReceptor ? 'error' : ''}
                                required={formData.tipo === 'Factura'}
                            />
                            {errors.rucReceptor && <div className="error-message">{errors.rucReceptor}</div>}
                        </div>

                        <div className="form-group">
                            <label className={formData.tipo === 'Factura' ? 'required' : ''}>Razón Social Receptor</label>
                            <input
                                type="text"
                                name="razonSocialReceptor"
                                value={formData.razonSocialReceptor}
                                onChange={handleChange}
                                placeholder="Nombre del cliente"
                                className={errors.razonSocialReceptor ? 'error' : ''}
                                required={formData.tipo === 'Factura'}
                            />
                            {errors.razonSocialReceptor && <div className="error-message">{errors.razonSocialReceptor}</div>}
                        </div>
                    </div>
                </div>

                {/* Items */}
                <div className="form-card items-section">
                    <div className="items-header">
                        <h3 className="form-section-title">Items del Comprobante</h3>
                        <button type="button" className="btn-add-item" onClick={addItem}>
                            <Plus size={20} />
                            Agregar Item
                        </button>
                    </div>

                    {formData.items.map((item, index) => (
                        <div key={index} className="item-card">
                            <div className="item-header">
                                <span className="item-number">Item #{index + 1}</span>
                                {formData.items.length > 1 && (
                                    <button
                                        type="button"
                                        className="btn-remove-item"
                                        onClick={() => removeItem(index)}
                                    >
                                        <Trash2 size={16} />
                                        Eliminar
                                    </button>
                                )}
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label className="required">Descripción</label>
                                    <input
                                        type="text"
                                        value={item.descripcion}
                                        onChange={(e) => handleItemChange(index, 'descripcion', e.target.value)}
                                        placeholder="Descripción del producto o servicio"
                                        className={errors[`item_${index}_descripcion`] ? 'error' : ''}
                                        required
                                    />
                                    {errors[`item_${index}_descripcion`] && (
                                        <div className="error-message">{errors[`item_${index}_descripcion`]}</div>
                                    )}
                                </div>

                                <div className="form-group">
                                    <label className="required">Cantidad</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        value={item.cantidad}
                                        onChange={(e) => handleItemChange(index, 'cantidad', e.target.value)}
                                        placeholder="0.00"
                                        min="0.01"
                                        className={errors[`item_${index}_cantidad`] ? 'error' : ''}
                                        required
                                    />
                                    {errors[`item_${index}_cantidad`] && (
                                        <div className="error-message">{errors[`item_${index}_cantidad`]}</div>
                                    )}
                                </div>

                                <div className="form-group">
                                    <label className="required">Precio Unitario</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        value={item.precioUnitario}
                                        onChange={(e) => handleItemChange(index, 'precioUnitario', e.target.value)}
                                        placeholder="0.00"
                                        min="0.01"
                                        className={errors[`item_${index}_precioUnitario`] ? 'error' : ''}
                                        required
                                    />
                                    {errors[`item_${index}_precioUnitario`] && (
                                        <div className="error-message">{errors[`item_${index}_precioUnitario`]}</div>
                                    )}
                                </div>
                            </div>

                            <div className="item-subtotal">
                                Subtotal: {formatCurrency(calculateItemSubtotal(item))}
                            </div>
                        </div>
                    ))}
                </div>

                {/* Totales */}
                <div className="totales-section">
                    <div className="total-row subtotal">
                        <span>Subtotal:</span>
                        <span>{formatCurrency(calculateSubtotal())}</span>
                    </div>
                    <div className="total-row igv">
                        <span>IGV (18%):</span>
                        <span>{formatCurrency(calculateIGV())}</span>
                    </div>
                    <div className="total-row total">
                        <span>TOTAL:</span>
                        <span>{formatCurrency(calculateTotal())}</span>
                    </div>
                </div>

                {/* Acciones */}
                <div className="form-actions">
                    <button
                        type="button"
                        className="btn-cancel"
                        onClick={onCancel}
                        disabled={loading}
                    >
                        Cancelar
                    </button>
                    <button
                        type="submit"
                        className="btn-submit"
                        disabled={loading}
                    >
                        {loading ? 'Guardando...' : 'Crear Comprobante'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ComprobanteForm;