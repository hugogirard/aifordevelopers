import { useLanguageContext } from "./languageContext";

const LanguageSelect = () => {
  const { languages, onClickLanguageChange } = useLanguageContext();
  return (
    <select
      style={{
        width: 100,
        position: "absolute",
        bottom: 10,
        left: 10,
        height: "30px",
      }}
      onChange={onClickLanguageChange}
    >
    {Object.keys(languages).map((lng) => (
        <option key={languages[lng as keyof typeof languages].nativeName} value={lng}>
            {languages[lng as keyof typeof languages].nativeName}
        </option>
    ))}
    </select>
  );
};

export default LanguageSelect;