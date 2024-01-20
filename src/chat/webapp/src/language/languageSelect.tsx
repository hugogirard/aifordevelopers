import { Dropdown, Option, makeStyles } from "@fluentui/react-components";
import { currentLanguage, useLanguageContext } from "./languageContext";

const useClasses = makeStyles({
  root: {
    minWidth: "150px",
    maxWidth: "150px",
    marginLeft: "10px",
    marginRight: "10px",
  }
});

const LanguageSelect = () => {
  const { languages, onClickLanguageChange } = useLanguageContext();
  const classes = useClasses();
  return (
    <Dropdown
      className={classes.root}
      defaultValue={languages[currentLanguage as keyof typeof languages].nativeName}
      defaultSelectedOptions={[currentLanguage]}
      onOptionSelect={(_,data) => { onClickLanguageChange(data.optionValue) }}>
        {Object.keys(languages).map((lng) => (
          <Option key={lng} value={lng}>
              {languages[lng as keyof typeof languages].nativeName}
          </Option>
        ))}
    </Dropdown>
  );
};

export default LanguageSelect;