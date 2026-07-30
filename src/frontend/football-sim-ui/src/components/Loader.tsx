import './Loader.css';

interface Props {
  size?: 'small' | 'medium' | 'large';
  text?: string;
}

export function Loader({ size = 'medium', text }: Props) {
  return (
    <div className={`loader-container loader-${size}`}>
      <div className="loader-spinner"></div>
      {text && <p className="loader-text">{text}</p>}
    </div>
  );
}
