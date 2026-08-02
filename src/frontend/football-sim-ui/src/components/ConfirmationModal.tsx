interface ConfirmationModalProps {
  title: string;
  message: string;
  confirmLabel: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmationModal({
  title,
  message,
  confirmLabel,
  onConfirm,
  onCancel,
}: ConfirmationModalProps) {
  return (
    <div className="confirmation-modal-backdrop" role="presentation" onClick={onCancel}>
      <section
        className="confirmation-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirmation-modal-title"
        onClick={(event) => event.stopPropagation()}
      >
        <h2 id="confirmation-modal-title">{title}</h2>
        <p>{message}</p>
        <div className="confirmation-modal-actions">
          <button type="button" className="game-menu-btn" onClick={onCancel}>
            Cancel
          </button>
          <button type="button" className="confirmation-modal-confirm-btn" onClick={onConfirm}>
            {confirmLabel}
          </button>
        </div>
      </section>
    </div>
  );
}
