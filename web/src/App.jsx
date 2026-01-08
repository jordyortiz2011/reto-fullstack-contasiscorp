import React, { useState } from 'react';
import { FileText, Plus, List } from 'lucide-react';
import ComprobanteList from './components/ComprobanteList';
import ComprobanteForm from './components/ComprobanteForm';
import ComprobanteDetail from './components/ComprobanteDetail';
import './App.css';

function App() {
    const [currentView, setCurrentView] = useState('list'); // 'list' | 'form'
    const [selectedComprobanteId, setSelectedComprobanteId] = useState(null);
    const [refreshKey, setRefreshKey] = useState(0);

    const handleViewDetail = (id) => {
        setSelectedComprobanteId(id);
    };

    const handleCloseDetail = () => {
        setSelectedComprobanteId(null);
    };

    const handleCreateSuccess = () => {
        setCurrentView('list');
        setRefreshKey(prev => prev + 1); // Trigger refresh
    };

    const handleCancel = () => {
        setCurrentView('list');
    };

    return (
        <div className="app">
            {/* Header */}
            <header className="app-header">
                <div className="header-content">
                    <div className="header-title">
                        <FileText size={32} />
                        <div>
                            <h1>Sistema de Comprobantes Electrónicos</h1>
                            <p>Gestión de Facturas y Boletas - SUNAT</p>
                        </div>
                    </div>
                    <div className="header-actions">
                        {currentView === 'list' ? (
                            <button
                                className="btn-primary"
                                onClick={() => setCurrentView('form')}
                            >
                                <Plus size={20} />
                                Nuevo Comprobante
                            </button>
                        ) : (
                            <button
                                className="btn-secondary"
                                onClick={() => setCurrentView('list')}
                            >
                                <List size={20} />
                                Ver Lista
                            </button>
                        )}
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="app-main">
                {currentView === 'list' ? (
                    <ComprobanteList
                        onViewDetail={handleViewDetail}
                        onRefresh={refreshKey}
                    />
                ) : (
                    <ComprobanteForm
                        onSuccess={handleCreateSuccess}
                        onCancel={handleCancel}
                    />
                )}
            </main>

            {/* Modal de Detalle */}
            {selectedComprobanteId && (
                <ComprobanteDetail
                    comprobanteId={selectedComprobanteId}
                    onClose={handleCloseDetail}
                />
            )}

            {/* Footer */}
            <footer className="app-footer">
                <p>© 2025 Contasiscorp - Sistema de Gestión de Comprobantes</p>
            </footer>
        </div>
    );
}

export default App;