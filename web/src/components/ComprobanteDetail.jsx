import React, { useState, useEffect } from 'react';
import { X, FileText, User, Building, Calendar, DollarSign } from 'lucide-react';
import comprobanteService from '../services/comprobanteService';
import { formatCurrency, formatDate, formatNumeroComprobante } from '../utils/formatters';
import '../styles/ComprobanteDetail.css';

const ComprobanteDetail = ({ comprobanteId, onClose }) => {
    const [comprobante, setComprobante] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        loadComprobante();
    }, [comprobanteId]);

    const loadComprobante = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await comprobanteService.getById(comprobanteId);
            setComprobante(data);
        } catch (err) {
            setError('Error al cargar el comprobante: ' + err.message);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="modal-overlay">
                <div className="modal-content">
                    <div className="loading">Cargando detalle...</div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="modal-overlay">
                <div className="modal-content">
                    <div className="error">{error}</div>
                    <button className="btn-close" onClick={onClose}>Cerrar</button>
                </div>
            </div>
        );
    }

    if (!comprobante) return null;

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <div className="modal-title">
                        <FileText size={24} />
                        <h2>Detalle del Comprobante</h2>
                    </div>
                    <button className="btn-close-icon" onClick={onClose}>
                        <X size={24} />
                    </button>
                </div>

                <div className="modal-body">
                    {/* Información Principal */}
                    <div className="detail-section">
                        <div className="detail-header">
              <span className={`badge-tipo-large ${comprobante.tipo.toLowerCase()}`}>
                {comprobante.tipo}
              </span>
                            <span className="numero-large">
                {formatNumeroComprobante(comprobante.serie, comprobante.numero)}
              </span>
                            <span className={`badge-large ${comprobante.estado === 'Emitido' ? 'badge-emitido' : 'badge-anulado'}`}>
                {comprobante.estado}
              </span>
                        </div>

                        <div className="detail-info-row">
                            <div className="detail-info-item">
                                <Calendar size={18} />
                                <div>
                                    <small>Fecha de Emisión</small>
                                    <strong>{formatDate(comprobante.fechaEmision)}</strong>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Emisor */}
                    <div className="detail-section">
                        <h3 className="section-title">
                            <Building size={20} />
                            Emisor
                        </h3>
                        <div className="empresa-detail">
                            <div className="empresa-field">
                                <label>RUC:</label>
                                <span>{comprobante.rucEmisor}</span>
                            </div>
                            <div className="empresa-field">
                                <label>Razón Social:</label>
                                <span>{comprobante.razonSocialEmisor}</span>
                            </div>
                        </div>
                    </div>

                    {/* Receptor */}
                    <div className="detail-section">
                        <h3 className="section-title">
                            <User size={20} />
                            Receptor
                        </h3>
                        {comprobante.rucReceptor ? (
                            <div className="empresa-detail">
                                <div className="empresa-field">
                                    <label>RUC:</label>
                                    <span>{comprobante.rucReceptor}</span>
                                </div>
                                <div className="empresa-field">
                                    <label>Razón Social:</label>
                                    <span>{comprobante.razonSocialReceptor}</span>
                                </div>
                            </div>
                        ) : (
                            <div className="text-muted-large">Consumidor Final</div>
                        )}
                    </div>

                    {/* Items */}
                    <div className="detail-section">
                        <h3 className="section-title">Items</h3>
                        <div className="items-table-container">
                            <table className="items-table">
                                <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Descripción</th>
                                    <th>Cantidad</th>
                                    <th>P. Unitario</th>
                                    <th>Subtotal</th>
                                </tr>
                                </thead>
                                <tbody>
                                {comprobante.items.map((item, index) => (
                                    <tr key={index}>
                                        <td>{index + 1}</td>
                                        <td>{item.descripcion}</td>
                                        <td>{item.cantidad}</td>
                                        <td>{formatCurrency(item.precioUnitario)}</td>
                                        <td className="monto">{formatCurrency(item.subtotal)}</td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>
                    </div>

                    {/* Totales */}
                    <div className="detail-section totales-detail">
                        <div className="total-detail-row">
                            <span>Subtotal:</span>
                            <span>{formatCurrency(comprobante.subTotal)}</span>
                        </div>
                        <div className="total-detail-row">
                            <span>IGV (18%):</span>
                            <span>{formatCurrency(comprobante.igv)}</span>
                        </div>
                        <div className="total-detail-row total">
                            <span>TOTAL:</span>
                            <span>{formatCurrency(comprobante.total)}</span>
                        </div>
                    </div>
                </div>

                <div className="modal-footer">
                    <button className="btn-close" onClick={onClose}>
                        Cerrar
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ComprobanteDetail;