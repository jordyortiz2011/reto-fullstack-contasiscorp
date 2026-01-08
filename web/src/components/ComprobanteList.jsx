import React, { useState, useEffect } from 'react';
import { Trash2, Eye, Filter, ChevronLeft, ChevronRight } from 'lucide-react';
import comprobanteService from '../services/comprobanteService';
import { formatCurrency, formatDate, formatNumeroComprobante } from '../utils/formatters';
import '../styles/ComprobanteList.css';

const ComprobanteList = ({ onViewDetail, onRefresh }) => {
    const [comprobantes, setComprobantes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({
        page: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0
    });

    // Filtros
    const [filters, setFilters] = useState({
        tipo: '',
        estado: '',
        fechaDesde: '',
        fechaHasta: '',
        rucReceptor: ''
    });

    const [showFilters, setShowFilters] = useState(false);

    useEffect(() => {
        loadComprobantes();
    }, [pagination.page, pagination.pageSize, onRefresh]);

    const loadComprobantes = async () => {
        try {
            setLoading(true);
            setError(null);

            const params = {
                page: pagination.page,
                pageSize: pagination.pageSize,
                ...(filters.tipo && { tipo: filters.tipo }),
                ...(filters.estado && { estado: filters.estado }),
                ...(filters.fechaDesde && { fechaDesde: filters.fechaDesde }),
                ...(filters.fechaHasta && { fechaHasta: filters.fechaHasta }),
                ...(filters.rucReceptor && { rucReceptor: filters.rucReceptor })
            };

            const data = await comprobanteService.getAll(params);
            setComprobantes(data.items);
            setPagination(prev => ({
                ...prev,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            }));
        } catch (err) {
            setError('Error al cargar los comprobantes: ' + err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleAnular = async (id) => {
        if (!window.confirm('¿Está seguro que desea anular este comprobante?')) {
            return;
        }

        try {
            await comprobanteService.anular(id);
            alert('Comprobante anulado exitosamente');
            loadComprobantes();
        } catch (err) {
            const errorMessage = err.response?.data?.detail || err.message;
            alert('Error al anular el comprobante: ' + errorMessage);
        }
    };

    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters(prev => ({ ...prev, [name]: value }));
    };

    const handleApplyFilters = () => {
        setPagination(prev => ({ ...prev, page: 1 }));
        loadComprobantes();
    };

    const handleClearFilters = () => {
        setFilters({
            tipo: '',
            estado: '',
            fechaDesde: '',
            fechaHasta: '',
            rucReceptor: ''
        });
        setPagination(prev => ({ ...prev, page: 1 }));
        setTimeout(loadComprobantes, 100);
    };

    const getEstadoBadgeClass = (estado) => {
        return estado === 'Emitido' ? 'badge-emitido' : 'badge-anulado';
    };

    if (loading) {
        return <div className="loading">Cargando comprobantes...</div>;
    }

    if (error) {
        return <div className="error">{error}</div>;
    }

    return (
        <div className="comprobante-list">
            <div className="list-header">
                <h2>Lista de Comprobantes</h2>
                <button
                    className="btn-filter"
                    onClick={() => setShowFilters(!showFilters)}
                >
                    <Filter size={20} />
                    {showFilters ? 'Ocultar Filtros' : 'Mostrar Filtros'}
                </button>
            </div>

            {showFilters && (
                <div className="filters-panel">
                    <div className="filter-row">
                        <div className="filter-field">
                            <label>Tipo</label>
                            <select name="tipo" value={filters.tipo} onChange={handleFilterChange}>
                                <option value="">Todos</option>
                                <option value="Factura">Factura</option>
                                <option value="Boleta">Boleta</option>
                            </select>
                        </div>

                        <div className="filter-field">
                            <label>Estado</label>
                            <select name="estado" value={filters.estado} onChange={handleFilterChange}>
                                <option value="">Todos</option>
                                <option value="Emitido">Emitido</option>
                                <option value="Anulado">Anulado</option>
                            </select>
                        </div>

                        <div className="filter-field">
                            <label>Fecha Desde</label>
                            <input
                                type="date"
                                name="fechaDesde"
                                value={filters.fechaDesde}
                                onChange={handleFilterChange}
                            />
                        </div>

                        <div className="filter-field">
                            <label>Fecha Hasta</label>
                            <input
                                type="date"
                                name="fechaHasta"
                                value={filters.fechaHasta}
                                onChange={handleFilterChange}
                            />
                        </div>

                        <div className="filter-field">
                            <label>RUC Receptor</label>
                            <input
                                type="text"
                                name="rucReceptor"
                                value={filters.rucReceptor}
                                onChange={handleFilterChange}
                                placeholder="11 dígitos"
                                maxLength="11"
                            />
                        </div>
                    </div>

                    <div className="filter-actions">
                        <button className="btn-apply" onClick={handleApplyFilters}>
                            Aplicar Filtros
                        </button>
                        <button className="btn-clear" onClick={handleClearFilters}>
                            Limpiar
                        </button>
                    </div>
                </div>
            )}

            <div className="table-container">
                <table className="comprobante-table">
                    <thead>
                    <tr>
                        <th>Tipo</th>
                        <th>Serie-Número</th>
                        <th>Fecha Emisión</th>
                        <th>Emisor</th>
                        <th>Receptor</th>
                        <th>Total</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                    </thead>
                    <tbody>
                    {comprobantes.length === 0 ? (
                        <tr>
                            <td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>
                                No se encontraron comprobantes
                            </td>
                        </tr>
                    ) : (
                        comprobantes.map((comprobante) => (
                            <tr key={comprobante.id}>
                                <td>
                    <span className={`badge-tipo ${comprobante.tipo.toLowerCase()}`}>
                      {comprobante.tipo}
                    </span>
                                </td>
                                <td className="numero">
                                    {formatNumeroComprobante(comprobante.serie, comprobante.numero)}
                                </td>
                                <td>{formatDate(comprobante.fechaEmision)}</td>
                                <td>
                                    <div className="empresa-info">
                                        <strong>{comprobante.razonSocialEmisor}</strong>
                                        <small>{comprobante.rucEmisor}</small>
                                    </div>
                                </td>
                                <td>
                                    {comprobante.rucReceptor ? (
                                        <div className="empresa-info">
                                            <strong>{comprobante.razonSocialReceptor}</strong>
                                            <small>{comprobante.rucReceptor}</small>
                                        </div>
                                    ) : (
                                        <span className="text-muted">Consumidor Final</span>
                                    )}
                                </td>
                                <td className="monto">{formatCurrency(comprobante.total)}</td>
                                <td>
                    <span className={`badge ${getEstadoBadgeClass(comprobante.estado)}`}>
                      {comprobante.estado}
                    </span>
                                </td>
                                <td>
                                    <div className="action-buttons">
                                        <button
                                            className="btn-icon btn-view"
                                            onClick={() => onViewDetail(comprobante.id)}
                                            title="Ver detalle"
                                        >
                                            <Eye size={18} />
                                        </button>
                                        {comprobante.estado === 'Emitido' && (
                                            <button
                                                className="btn-icon btn-delete"
                                                onClick={() => handleAnular(comprobante.id)}
                                                title="Anular"
                                            >
                                                <Trash2 size={18} />
                                            </button>
                                        )}
                                    </div>
                                </td>
                            </tr>
                        ))
                    )}
                    </tbody>
                </table>
            </div>

            {comprobantes.length > 0 && (
                <div className="pagination">
                    <div className="pagination-info">
                        Mostrando {comprobantes.length} de {pagination.totalCount} comprobantes
                    </div>
                    <div className="pagination-controls">
                        <button
                            className="btn-page"
                            onClick={() => setPagination(prev => ({ ...prev, page: prev.page - 1 }))}
                            disabled={pagination.page === 1}
                        >
                            <ChevronLeft size={20} />
                            Anterior
                        </button>
                        <span className="page-number">
              Página {pagination.page} de {pagination.totalPages || 1}
            </span>
                        <button
                            className="btn-page"
                            onClick={() => setPagination(prev => ({ ...prev, page: prev.page + 1 }))}
                            disabled={pagination.page >= pagination.totalPages}
                        >
                            Siguiente
                            <ChevronRight size={20} />
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ComprobanteList;