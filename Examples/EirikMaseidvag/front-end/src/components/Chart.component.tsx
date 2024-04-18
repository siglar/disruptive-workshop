import { FC } from "react";
import { LineChart, Line, XAxis, YAxis, Tooltip, TooltipProps } from "recharts";
import { TemperatureViewModel } from "../models/temperature.viewmodel";
import {
  ValueType,
  NameType,
} from "recharts/types/component/DefaultTooltipContent";

interface ChartProps {
  values: TemperatureViewModel[];
}

const Chart: FC<ChartProps> = (props: ChartProps) => {
  const renderTooltip = (content: TooltipProps<ValueType, NameType>) => {
    if (content.payload && content.payload.length > 0) {
      return (
        <article
          style={{
            border: "#bbb 1.5px solid",
          }}
        >
          <p
            style={{
              margin: "0 0",
              padding: "3px 7.5px",
              borderBottom: "#bbb 1.5px solid",
            }}
          >
            {content.payload[0].payload.temperature}°C
          </p>
        </article>
      );
    }
    return null;
  };

  const formatDate = (date: string) => {
    const currentDate = new Date(date);

    return `${currentDate.getHours()}:${currentDate.getMinutes()}:${currentDate.getSeconds()}`;
  };

  return (
    <LineChart width={1000} height={300} data={props.values}>
      <Line
        animationEasing={"linear"}
        animationDuration={250}
        type="monotone"
        dataKey="temperature"
        stroke="#8884d8"
      />
      <Tooltip content={(content) => renderTooltip(content)} />
      <XAxis dataKey="timestamp" tickFormatter={formatDate} />
      <YAxis />
    </LineChart>
  );
};

export default Chart;
